//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
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


using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Serialization;
using Canyala.Lagoon.Text;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Test;

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

    static readonly double[][] JaggedArrayData =
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

        Assert.AreEqual(message, deserialized);
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
    }

    [TestMethod]
    public void ItShouldBePossibleToSerializeStatelessMessages()
    {
        var message = new EmptyMessage();
        var serialized = Json.Serialize(message);
        var deserialized = Json.Deserialize<EmptyMessage>(serialized);

        Assert.AreEqual(message, deserialized);
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

        Assert.AreEqual(messages, deserialized);
    }

    [TestMethod]
    public void ImmidiateArraySerialization()
    {
        var array = new int[] { 5, 3, 5 };
        var serialized = Json.Serialize(array);
        var deserialized = Json.Deserialize<int[]>(serialized);

        Assert.AreEqual(array, deserialized);
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

        Assert.AreEqual(number, deserialized);
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

        Assert.AreEqual(samples, deserialized);
    }

    public class RegisterModel
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Department { get; set; }
        public string? Mail { get; set; }
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

        Assert.AreEqual(registerModelArray, deserialized);
    }
}
