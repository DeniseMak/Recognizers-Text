﻿using Microsoft.Recognizers.Text.DateTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Recognizers.Text.DataDrivenTests
{
    [TestClass]
    public class TestBase
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        public IModel Model { get; set; }
        public IExtractor Extractor { get; set; }
        public IDateTimeParser DateTimeParser { get; set; }
        public TestModel TestSpec { get; set; }

        public void TestSpecInitialize(TestResources resources)
        {
            TestSpec = resources.GetSpecForContext(TestContext);
        }

        public void ModelInitialize(IDictionary<string, IModel> models)
        {
            var key = TestContext.TestName;
            IModel model;
            if (!models.TryGetValue(key, out model))
            {
                model = TestContext.GetModel();
                models.Add(key, model);
            }

            Model = model;
        }

        public void ExtractorInitialize(IDictionary<string, IExtractor> extractors)
        {
            var key = TestContext.TestName;
            IExtractor extractor;
            if (!extractors.TryGetValue(key, out extractor))
            {
                extractor = TestContext.GetExtractor();
                extractors.Add(key, extractor);
            }

            Extractor = extractor;
        }

        public void ParserInitialize(IDictionary<string, IDateTimeParser> parsers)
        {
            var key = TestContext.TestName;
            IDateTimeParser parser;
            if (!parsers.TryGetValue(key, out parser))
            {
                parser = TestContext.GetDateTimeParser();
                parsers.Add(key, parser);
            }

            DateTimeParser = parser;
        }

        public void TestNumber()
        {
            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message);
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var actualResults = Model.Parse(TestSpec.Input);
            var expectedResults = TestSpec.CastResults<ModelResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count, GetMessage(TestSpec));

            foreach (var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;

                Assert.AreEqual(expected.TypeName, actual.TypeName, GetMessage(TestSpec));
                Assert.AreEqual(expected.Text, actual.Text, GetMessage(TestSpec));
                Assert.AreEqual(expected.Resolution["value"], actual.Resolution["value"], GetMessage(TestSpec));
            }
        }

        public void TestNumberWithUnit()
        {

            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message);
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var actualResults = Model.Parse(TestSpec.Input);
            var expectedResults = TestSpec.CastResults<ModelResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count, GetMessage(TestSpec));

            foreach (var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;

                Assert.AreEqual(expected.TypeName, actual.TypeName, GetMessage(TestSpec));
                Assert.AreEqual(expected.Text, actual.Text, GetMessage(TestSpec));
                Assert.AreEqual(expected.Resolution["value"], actual.Resolution["value"], GetMessage(TestSpec));
                Assert.AreEqual(expected.Resolution["unit"], actual.Resolution["unit"], GetMessage(TestSpec));
            }
        }

        public void TestDateTime()
        {

            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message);
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var referenceDateTime = TestSpec.GetReferenceDateTime();

            var actualResults = ((DateTimeModel)Model).Parse(TestSpec.Input, referenceDateTime);
            var expectedResults = TestSpec.CastResults<ModelResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count, GetMessage(TestSpec));

            foreach (var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;

                Assert.AreEqual(expected.TypeName, actual.TypeName, GetMessage(TestSpec));
                Assert.AreEqual(expected.Text, actual.Text, GetMessage(TestSpec));
                
                var values = actual.Resolution as IDictionary<string, object>;
                var listValues = values["values"] as IList<Dictionary<string, string>>;
                var actualValues = listValues.FirstOrDefault();

                var expectedObj = JsonConvert.DeserializeObject<IList<Dictionary<string, string>>>(expected.Resolution["values"].ToString());
                var expectedValues = expectedObj.FirstOrDefault();

                CollectionAssert.AreEqual(expectedValues, actualValues, GetMessage(TestSpec));
            }
        }

        public void TestDateTimeExtractor()
        {

            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message);
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var actualResults = Extractor.Extract(TestSpec.Input);
            var expectedResults = TestSpec.CastResults<ExtractResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count, GetMessage(TestSpec));

            foreach (var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;

                Assert.AreEqual(expected.Type, actual.Type, GetMessage(TestSpec));
                Assert.AreEqual(expected.Text, actual.Text, GetMessage(TestSpec));
            }
        }

        public void TestDateTimeParser()
        {

            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message, GetMessage(TestSpec));
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var referenceDateTime = TestSpec.GetReferenceDateTime();

            var extractResults = Extractor.Extract(TestSpec.Input);
            var actualResults = extractResults.Select(o => DateTimeParser.Parse(o, referenceDateTime)).ToArray();

            var expectedResults = TestSpec.CastResults<DateTimeParseResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count(), GetMessage(TestSpec));

            foreach(var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;
                Assert.AreEqual(expected.Type, actual.Type, GetMessage(TestSpec));
                Assert.AreEqual(expected.Text, actual.Text, GetMessage(TestSpec));

                var actualValue = actual.Value as DateTimeResolutionResult;
                var expectedValue = JsonConvert.DeserializeObject<DateTimeResolutionResult>(expected.Value.ToString());

                Assert.IsNotNull(actualValue, GetMessage(TestSpec));
                Assert.AreEqual(expectedValue.Timex, actualValue.Timex, GetMessage(TestSpec));
                CollectionAssert.AreEqual(expectedValue.FutureResolution, actualValue.FutureResolution, GetMessage(TestSpec));
                CollectionAssert.AreEqual(expectedValue.PastResolution, actualValue.PastResolution, GetMessage(TestSpec));
            }
        }

        public void TestDateTimeMergedParser()
        {

            string message;
            if (TestUtils.EvaluateSpec(TestSpec, out message))
            {
                Assert.Inconclusive(message);
            }

            if (Debugger.IsAttached && TestSpec.Debug)
            {
                Debugger.Break();
            }

            var referenceDateTime = TestSpec.GetReferenceDateTime();

            var extractResults = Extractor.Extract(TestSpec.Input);
            var actualResults = extractResults.Select(o => DateTimeParser.Parse(o, referenceDateTime)).ToArray();
            
            var expectedResults = TestSpec.CastResults<DateTimeParseResult>();

            Assert.AreEqual(expectedResults.Count(), actualResults.Count(), GetMessage(TestSpec));

            foreach (var tuple in Enumerable.Zip(expectedResults, actualResults, Tuple.Create))
            {
                var expected = tuple.Item1;
                var actual = tuple.Item2;

                var values = actual.Value as IDictionary<string, object>;
                if (values != null)
                {
                    var actualValues = values["values"] as IList<Dictionary<string, string>>;

                    var expectedObj = JsonConvert.DeserializeObject<IDictionary<string, IList<Dictionary<string, string>>>>(expected.Value.ToString());
                    var expectedValues = expectedObj["values"];

                    foreach (var results in Enumerable.Zip(expectedValues, actualValues, Tuple.Create))
                    {
                        CollectionAssert.AreEqual(results.Item1, results.Item2, GetMessage(TestSpec));
                    }
                }
            }
        }

        private static string GetMessage(TestModel spec)
        {
            return $"Input: \"{spec.Input}\"";
        }
    }
}
