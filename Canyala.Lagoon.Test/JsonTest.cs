using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Serialization;
using Canyala.Lagoon.Text;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class JsonTest
    {
        static string[] examples = 
        {
            "{\"$Person\":{\"a\":1,\"b\":2},\"Animal\":\"Tiger\"}",
            "{\"Data1\":[1,2,3,4,5,{\"X\":\"Funny\"}],\"Data2\":[7,8,9,10]}",
            "{\"$Person\":{\"FirstName\":\"Mar\\\"tin\",\"LastName\":\"Fredr\\\niksson\"}}",
        };

        [TestMethod]
        public void RoundtripWithEscapesEtc()
        { 
            examples.Do(example => Assert.AreEqual(example, Json.Object.Parse(new SubString(example)).ToString())); 
        }

        class JaggedArrayMessage
        {
            public readonly double[][] Data;
            public readonly DateTime StartDate;
            public readonly DateTimeOffset StartEvent;
            public readonly Guid MessageId;

            private JaggedArrayMessage(double[][] data, DateTime startDate, DateTimeOffset startEvent, Guid messageId)
            { 
                Data = data;
                StartDate = startDate;
                StartEvent = startEvent;
                MessageId = messageId;
            }

            internal static JaggedArrayMessage FromData(double[][] data, DateTime startDate, DateTimeOffset startEvent, Guid messageId)
            { return new JaggedArrayMessage(data, startDate, startEvent, messageId); }
        }

        static double[][] JaggedArrayData =
        {
            new double[] { 1.0, 2.0 },
            new double[] { 3.0, 4.0 }
        };

        [TestMethod]
        public void SerializeAndDeserializeOfJaggedArrays()
        {
            var message = JaggedArrayMessage.FromData(JaggedArrayData, DateTime.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());
            var serialized = Json.Serialize(message);
            var deserialized = Json.Deserialize<JaggedArrayMessage>(serialized);
        }

        class ArrayMessage
        {
            public readonly string Name;
            public readonly double[,] Data;
            public readonly TimeSpan ElapsedTime;
            public readonly bool State1;
            public readonly bool State2;

            private ArrayMessage(string name, double[,] data, TimeSpan elapsedTime, bool state1, bool state2)
            {
                Name = name;
                Data = data;
                ElapsedTime = elapsedTime;

                State1 = state1;
                State2 = state2;
            }

            internal static ArrayMessage FromData(string name, double[,] data, TimeSpan elapsedTime, bool state1, bool state2)
                { return new ArrayMessage(name, data, elapsedTime, state1, state2); }
        }

        static double[,] ArrayData =
        {
            { 1.0, 2.0 },
            { 3.0, 4.0 }
        };

        [TestMethod]
        public void SerializAndDeserializeOfMultidimensionalArrays()
        {
            var message = ArrayMessage.FromData("Martin\nFredriksson", ArrayData, TimeSpan.FromMilliseconds(1876.23), true, false);
            var serialized = Json.Serialize(message);
            var deserialized = Json.Deserialize<ArrayMessage>(serialized);
        }

        class EmptyMessage
        {
        }

        [TestMethod]
        public void ItShouldBePossibleToSerializeStatelessMessages()
        {
            var message = new EmptyMessage();
            var serialized = Json.Serialize(message);
            var deserialized = Json.Deserialize<EmptyMessage>(serialized);
        }

        public class InnerMessage
        {
            public readonly string Inner;

            protected InnerMessage(string inner)
                { Inner = inner; }

            public static InnerMessage FromText(string text)
                { return new InnerMessage(text); }
        }

        public class DerivedInnerMessage : InnerMessage
        {
            public readonly string DerivedInner;

            private DerivedInnerMessage(string derivedInner, string inner) : base(inner)
                { DerivedInner = derivedInner; }

            public static DerivedInnerMessage FromText(string derivedInner, string inner)
                { return new DerivedInnerMessage(derivedInner, inner); }
        }

        public class OuterMessage
        {
            public readonly InnerMessage[] InnerMessageArray;
            public readonly InnerMessage InnerMessage;
            public readonly string Name;
            public readonly int Value;

            private OuterMessage(InnerMessage[] innerMessageArray, InnerMessage innerMessage, string name, int value)
            {
                InnerMessageArray = innerMessageArray;
                InnerMessage = innerMessage;
                Value = value;
                Name = name;
            }

            public static OuterMessage FromInnerMessageEtc(string text, string name, int value)
            {
                InnerMessage[] innerMessageArray = new InnerMessage[2];
                innerMessageArray[0] = DerivedInnerMessage.FromText("Hipp", "happ");
                innerMessageArray[1] = InnerMessage.FromText("tjipp");

                return new OuterMessage(innerMessageArray, DerivedInnerMessage.FromText("derived data", text), name, value);
            }
        }

        [TestMethod]
        public void PolymorphicInnerAndOuterSerialization()
        {
            var outerMessageWithInner = OuterMessage.FromInnerMessageEtc("anInner", "aName", 3);
            var serialized = Json.Serialize(outerMessageWithInner);
            var deserialized = Json.Deserialize<OuterMessage>(serialized);
        }

        [TestMethod]
        public void ArrayOfComposites()
        {
            var messages = new [] {
                OuterMessage.FromInnerMessageEtc("anInner", "aName", 3),
                OuterMessage.FromInnerMessageEtc("Kalle", "Hepp", 4)
            };

            var serialized = Json.Serialize(messages);
            var deserialized = Json.Deserialize<OuterMessage[]>(serialized);
        }
    
        [TestMethod]
        public void ImmidiateArraySerialization()
        {
            var array = new int[] { 5, 3, 5 };
            var serialized = Json.Serialize(array);
            var deserialized = Json.Deserialize<int[]>(serialized);
        }

        [TestMethod]
        public void EvenSingleBaseTypesShouldWorkNow()
        {
            double pi = 3.14159;
            var serialized = Json.Serialize(pi);
            var deserialized = Json.Deserialize<double>(serialized);
        }

        public class Person
        {
            public readonly string Name;
            public readonly int Age;

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }

        public class Employee : Person
        {
            public readonly string Apartment;

            public Employee(string name, int age, string apartment)
                :base(name,age)
            {
                Apartment = apartment;
            }
        }

        [TestMethod]
        public void TopLevelPolymorphismWithoutTypeInfo()
        {
            var person = new Person("John", 24);
            var serialized = Json.Serialize(person);
            var deserialized = Json.Deserialize<Person>(serialized);
        }

        [TestMethod]
        public void TopLevelPolymorphismWithTypeInfo()
        {
            var person = new Employee("Steve", 25, "Development") as Person;
            var serialized = Json.Serialize(person);
            var deserialized = Json.Deserialize<Person>(serialized);
        }

        public class Generic<T>
        {
            public readonly T Property;

            public Generic(T property)
            {
                Property = property;
            }

        }

        [TestMethod]
        public void SerializeGenerics()
        {
            var generic = new Generic<string>("Hello");
            var serialized = Json.Serialize(generic);
            var deserialized = Json.Deserialize<Generic<string>>(serialized);
            Assert.AreEqual(generic.Property, deserialized.Property);
        }

        [TestMethod]
        public void TypeSerialization()
        {
            Type type = typeof(OuterMessage);
            var serialized = Json.Serialize(type);
            var deserialized = Json.Deserialize<Type>(serialized);
            Assert.AreEqual(type, deserialized);
        }

        enum Numbers { One, Two, Three, Four };

        [TestMethod]
        public void EnumSerialization()
        {
            var number = Numbers.Three;
            var serialized = Json.Serialize(number);
            var deserialized = Json.Deserialize<Numbers>(serialized);
        }

        class Base
        {
        }

        class Derived : Base
        {
            public readonly DateTime TimeStamp;
            public readonly double Value;

            public Derived(DateTime timeStamp, double value)
            {
                TimeStamp = timeStamp;
                Value = value;
            }
        }

        class Message
        {
            public readonly Base[] Samples;

            public Message(Base[] samples)
            {
                Samples = samples;
            }
        }

        [TestMethod]
        public void TupleSerialize()
        {
            var sample = Tuple.Create(DateTime.UtcNow, 3.14);
            var samples = Seq.Array(sample, sample);
            var serialized = Json.Serialize(samples);
            var deserialized = Json.Deserialize<Tuple<DateTime,double>[]>(serialized);
        }

        public class RegisterModel
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Department { get; set; }
            public string Mail { get; set; }
        }

        [TestMethod]
        public void DataObjectSerialize()
        {
            var registerModel = new RegisterModel
            {
                Company = "TestCompany",
                Department = "TestDepartment",
                Mail = "TestMail",
                Name = "TestName"
            };

            var serialized = Json.Serialize(registerModel);
            var deserialized = Json.Deserialize<RegisterModel>(serialized);
        }

        [TestMethod]
        public void DataObjectArraySerialize()
        {
            var registerModel = new RegisterModel
            {
                Company = "TestCompany",
                Department = "TestDepartment",
                Mail = "TestMail",
                Name = "TestName"
            };

            var serialized = Json.Serialize(Seq.Array(registerModel));
            var deserialized = Json.Deserialize<RegisterModel[]>(serialized);
        }
    }
}
