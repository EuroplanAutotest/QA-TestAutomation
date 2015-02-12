using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using QA.TestAutomation.Framework.Attributes;
using TechTalk.SpecFlow;

namespace QA.TestAutomation.Framework.Tests
{
    [TestFixture]
    public class GherkinGenerationTestsBase : TestsBase
    {
        #region SpecFLow setup

        private static ITestRunner testRunner;

        [TestFixtureSetUp]
        public virtual void FeatureSetup()
        {
            testRunner = TestRunnerManager.GetTestRunner();
            var features = GetType().GetCustomAttributes(typeof (FeatureAttribute), false);
            if (!features.Any())
            {
                throw new ConfigurationErrorsException("Feature Attribute is required");
            }

            var feature = (FeatureAttribute)features.Single();
            var featureInfo = new FeatureInfo(
                new System.Globalization.CultureInfo("en-US"),
                feature.Title,
                feature.Story,
                ProgrammingLanguage.CSharp, null);

            testRunner.OnFeatureStart(featureInfo);
        }

        [TestFixtureTearDown]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }

        #endregion

        #region SetUp & TearDown

        public override void SetUp()
        {
            try
            {
                var testName = TestContext.CurrentContext.Test.Name.Split('(')[0];
                var scenario = (Scenario)GetType().GetMethod(testName).GetCustomAttributes(typeof(Scenario), false).Single();
                var scenarioInfo = new ScenarioInfo(scenario.Title, scenario.Tags);
                ScenarioSetup(scenarioInfo);
            }
            catch (InvalidOperationException e)
            {
                
                throw new ConfigurationErrorsException("You must provide Scenario attribute", e);
            }

        }
        
        [TearDown]
        public virtual void TearDown()
        {
            ScenarioCleanup();
            testRunner.OnScenarioEnd();
        }

        #endregion

        protected void ScenarioSetup(ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }

        protected void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }

        protected void Given(string given, string keyword = "Given ")
        {
            testRunner.Given(given, null, null, keyword);
        }

        protected void When(string when, string keyword = "When ")
        {
            testRunner.When(when, null, null, keyword);
        }

        protected void Then(string then, string keyword = "Then ")
        {
            testRunner.Then(then, null, null, keyword);
        }
    }
}
