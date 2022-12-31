//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 


using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Serialization;
using Canyala.Lagoon.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Canyala.Lagoon.Test.All;

[TestClass]
public class JsonTest
{
    static readonly string[] examples = 
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

    class JaggedArrayMessage : IEquatable<JaggedArrayMessage?>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as JaggedArrayMessage);
        }

        public bool Equals(JaggedArrayMessage? other)
        {
            return other is not null &&
                   Data.SequenceOfSequencesEqual(other.Data) &&
                   StartDate == other.StartDate &&
                   StartEvent.Equals(other.StartEvent) &&
                   MessageId.Equals(other.MessageId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, StartDate, StartEvent, MessageId);
        }

        public static bool operator ==(JaggedArrayMessage? left, JaggedArrayMessage? right)
        {
            return EqualityComparer<JaggedArrayMessage>.Default.Equals(left, right);
        }

        public static bool operator !=(JaggedArrayMessage? left, JaggedArrayMessage? right)
        {
            return !(left == right);
        }
    }

    static readonly double[][] JaggedArrayData =
    {
        new double[] { 1.0, 2.0 },
        new double[] { 3.0, 4.0 }
    };

    [TestMethod]
    public void SerializeAndDeserializeOfJaggedArraysWithUtcTimes()
    {
        var message = JaggedArrayMessage.FromData(JaggedArrayData, DateTime.UtcNow, DateTimeOffset.UtcNow, Guid.NewGuid());
        var serialized = Json.Serialize(message);
        var deserialized = Json.Deserialize<JaggedArrayMessage>(serialized);

        Assert.AreEqual(message, deserialized);
    }

    [TestMethod]
    public void SerializeAndDeserializeOfJaggedArraysWithLocalTimes()
    {
        var message = JaggedArrayMessage.FromData(JaggedArrayData, DateTime.Now, DateTimeOffset.Now, Guid.NewGuid());
        var serialized = Json.Serialize(message);
        var deserialized = Json.Deserialize<JaggedArrayMessage>(serialized);

        Assert.AreEqual(message, deserialized);
    }

    class ArrayMessage : IEquatable<ArrayMessage?>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as ArrayMessage);
        }

        public bool Equals(ArrayMessage? other)
        {
            return other is not null &&
                   Name == other.Name &&
                   Data.ArrayEquals(other.Data) &&
                   ElapsedTime.Equals(other.ElapsedTime) &&
                   State1 == other.State1 &&
                   State2 == other.State2;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Data, ElapsedTime, State1, State2);
        }

        public static bool operator ==(ArrayMessage? left, ArrayMessage? right)
        {
            return EqualityComparer<ArrayMessage>.Default.Equals(left, right);
        }

        public static bool operator !=(ArrayMessage? left, ArrayMessage? right)
        {
            return !(left == right);
        }
    }

    static readonly double[,] ArrayData =
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

        Assert.AreEqual(message, deserialized);
    }

    class EmptyMessage 
    {
        public override bool Equals(object? obj)
        {
            return Equals(obj as EmptyMessage);
        }

        public bool Equals(EmptyMessage? other)
        {
            return other is not null;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    [TestMethod]
    public void ItShouldBePossibleToSerializeStatelessMessages()
    {
        var message = new EmptyMessage();
        var serialized = Json.Serialize(message);
        var deserialized = Json.Deserialize<EmptyMessage>(serialized);

        bool result = message.Equals(deserialized);

        Assert.AreEqual(message, deserialized);
    }

    public class InnerMessage
    {
        public readonly string Inner;

        protected InnerMessage(string inner)
            { Inner = inner; }

        public static InnerMessage FromText(string text)
            { return new InnerMessage(text); }

        public override bool Equals(object? obj)
        {
            return obj is InnerMessage message &&
                   Inner == message.Inner;
        }

        public override int GetHashCode()
        {
            return Inner.GetHashCode();       
        }
    }

    public class DerivedInnerMessage : InnerMessage
    {
        public readonly string DerivedInner;

        private DerivedInnerMessage(string derivedInner, string inner) : base(inner)
            { DerivedInner = derivedInner; }

        public static DerivedInnerMessage FromText(string derivedInner, string inner)
            { return new DerivedInnerMessage(derivedInner, inner); }

        public override bool Equals(object? obj)
        {
            return obj is DerivedInnerMessage message && base.Equals(obj) &&
               DerivedInner == message.DerivedInner;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Inner, DerivedInner);
        }
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

        public override bool Equals(object? obj)
        {
            return obj is OuterMessage message &&
                   InnerMessageArray.SequenceEqual(message.InnerMessageArray) &&
                   InnerMessage.Equals(message.InnerMessage) &&
                   Name == message.Name &&
                   Value == message.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InnerMessageArray.SequenceHashCode(InnerMessageArray.Length), InnerMessage , Value, Name);
        }

    }

    [TestMethod]
    public void PolymorphicInnerAndOuterSerialization()
    {
        var outerMessageWithInner = OuterMessage.FromInnerMessageEtc("anInner", "aName", 3);
        var serialized = Json.Serialize(outerMessageWithInner);
        var deserialized = Json.Deserialize<OuterMessage>(serialized);

        Assert.AreEqual(outerMessageWithInner, deserialized);
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

        CollectionAssert.AreEqual(messages, deserialized);
    }

    [TestMethod]
    public void ImmidiateArraySerialization()
    {
        var array = new int[] { 5, 3, 5 };
        var serialized = Json.Serialize(array);
        var deserialized = Json.Deserialize<int[]>(serialized);

        CollectionAssert.AreEqual(array, deserialized);
    }

    [TestMethod]
    public void EvenSingleBaseTypesShouldWorkNow()
    {
        double pi = 3.14159;
        var serialized = Json.Serialize(pi);
        var deserialized = Json.Deserialize<double>(serialized);

        Assert.AreEqual(pi, deserialized);
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

        public override bool Equals(object? obj)
        {
            return obj is Person person &&
                   Name == person.Name &&
                   Age == person.Age;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }
    }

    public class Employee : Person
    {
        public readonly string Apartment;

        public Employee(string name, int age, string apartment)
            : base(name, age)
        {
            Apartment = apartment;
        }

        public override bool Equals(object? obj)
        {
            return obj is Employee employee &&
               Apartment == employee.Apartment &&
               base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Apartment);
        }
    }

    [TestMethod]
    public void TopLevelPolymorphismWithoutTypeInfo()
    {
        var person = new Person("John", 24);
        var serialized = Json.Serialize(person);
        var deserialized = Json.Deserialize<Person>(serialized);

        Assert.AreEqual(person, deserialized);
    }

    [TestMethod]
    public void TopLevelPolymorphismWithTypeInfo()
    {
        var person = new Employee("Steve", 25, "Development") as Person;
        var serialized = Json.Serialize(person);
        var deserialized = Json.Deserialize<Person>(serialized);

        Assert.AreEqual(person, deserialized);
    }

    public class Generic<T>
    {
        public readonly T Property;

        public Generic(T property)
        {
            Property = property;
        }

        public override bool Equals(object? obj)
        {
            return obj is Generic<T> generic &&
                   EqualityComparer<T>.Default.Equals(Property, generic.Property);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property);
        }
    }

    [TestMethod]
    public void SerializeGenerics()
    {
        var generic = new Generic<string>("Hello");
        var serialized = Json.Serialize(generic);
        var deserialized = Json.Deserialize<Generic<string>>(serialized);

        Assert.AreEqual(generic.Property, deserialized?.Property);
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

        Assert.AreEqual(number, deserialized);
    }

    class Base
    {
    }

    class Derived : Base, IEquatable<Derived?>
    {
        public readonly DateTime TimeStamp;
        public readonly double Value;

        public Derived(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Derived);
        }

        public bool Equals(Derived? other)
        {
            return other is not null &&
                   TimeStamp == other.TimeStamp &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TimeStamp, Value);
        }

        public static bool operator ==(Derived? left, Derived? right)
        {
            return left is not null && left.Equals(right);
        }

        public static bool operator !=(Derived? left, Derived? right)
        {
            return !(left == right);
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

        CollectionAssert.AreEqual(samples, deserialized);
    }

    public class RegisterModel : IEquatable<RegisterModel?>
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Department { get; set; }
        public string? Mail { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RegisterModel);
        }

        public bool Equals(RegisterModel? other)
        {
            return other is not null &&
                   Name == other.Name &&
                   Company == other.Company &&
                   Department == other.Department &&
                   Mail == other.Mail;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Company, Department, Mail);
        }

        public static bool operator ==(RegisterModel? left, RegisterModel? right)
        {
            return left is not null && left.Equals(right);
        }

        public static bool operator !=(RegisterModel? left, RegisterModel? right)
        {
            return !(left == right);
        }
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

        Assert.AreEqual(registerModel, deserialized);
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

        var registerModelArray = Seq.Array(registerModel);
        var serialized = Json.Serialize(registerModelArray);
        var deserialized = Json.Deserialize<RegisterModel[]>(serialized);

        CollectionAssert.AreEqual(registerModelArray, deserialized);
    }
}
