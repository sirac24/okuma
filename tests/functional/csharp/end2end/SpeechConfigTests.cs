//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.CognitiveServices.Speech.Tests.EndToEnd
{
    using static Config;
    using static SpeechRecognitionTestsHelper;

    [TestClass]
    public class SpeechConfigTests : RecognitionTestBase
    {
        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            BaseClassInit(context);
        }

        [TestMethod]
        public void TestCreateRecognizerTypes()
        {
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather.AudioFile);
            using (var speechRecognizer = new SpeechRecognizer(this.config, audioInput))
            {
                Assert.IsInstanceOfType(speechRecognizer, typeof(SpeechRecognizer));
            }

            var sourceLanguage = Language.EN;
            var translatorConfig = SpeechTranslationConfig.FromSubscription(subscriptionKey, region);
            translatorConfig.SpeechRecognitionLanguage = sourceLanguage;

            var targetLanguages = new List<string>() { Language.DE, Language.ES };
            targetLanguages.ForEach(l => translatorConfig.AddTargetLanguage(l));

            using (var translationRecognizer = new TranslationRecognizer(translatorConfig, audioInput))
            {
                Assert.IsInstanceOfType(translationRecognizer, typeof(TranslationRecognizer));
            }

            using (var intentRecognizer = new IntentRecognizer(this.config, audioInput))
            {
                Assert.IsInstanceOfType(intentRecognizer, typeof(IntentRecognizer));
            }
        }

        [TestMethod]
        public void TestGetters()
        {
            Assert.AreEqual(subscriptionKey, this.config.SubscriptionKey);
            Assert.AreEqual(region, this.config.Region);
        }

        [TestMethod]
        public async Task ConfigFromAuthorizationToken()
        {
            var token = await Config.GetToken(subscriptionKey, region);
            var configWithToken = SpeechConfig.FromAuthorizationToken(token, region);

            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather.AudioFile);
            using (var speechRecognizer = new SpeechRecognizer(configWithToken, audioInput))
            {
                SpeechRecognitionTestsHelper helper = new SpeechRecognitionTestsHelper();

                Assert.AreEqual(token, speechRecognizer.AuthorizationToken);
                AssertMatching(TestData.English.Weather.Utterance, await helper.GetFirstRecognizerResult(speechRecognizer));
            }
        }

        [TestMethod]
        public async Task SetAuthorizationTokenOnConfig()
        {
            var invalidToken = "InvalidToken";
            var configWithToken = SpeechConfig.FromAuthorizationToken(invalidToken, region);
            Assert.AreEqual(invalidToken, configWithToken.AuthorizationToken);

            var newToken = await Config.GetToken(subscriptionKey, region);
            configWithToken.AuthorizationToken = newToken;
            Assert.AreEqual(newToken, configWithToken.AuthorizationToken);

            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather.AudioFile);
            using (var speechRecognizer = new SpeechRecognizer(configWithToken, audioInput))
            {
                SpeechRecognitionTestsHelper helper = new SpeechRecognitionTestsHelper();
                Assert.AreEqual(newToken, speechRecognizer.AuthorizationToken);
                AssertMatching(TestData.English.Weather.Utterance, await helper.GetFirstRecognizerResult(speechRecognizer));
            }
        }

        [TestMethod]
        public async Task SubscriptionKeyHasPrecedenceOverAuthorizationToken()
        {
            var invalidToken = "InvalidToken";
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather.AudioFile);
            // Create recognizer using subscription key.
            using (var speechRecognizer = new SpeechRecognizer(this.config, audioInput))
            {
                speechRecognizer.AuthorizationToken = invalidToken;

                SpeechRecognitionTestsHelper helper = new SpeechRecognitionTestsHelper();

                Assert.AreEqual(invalidToken, speechRecognizer.AuthorizationToken);
                // The recognizer should still work since non-empty subscription key has precedence over authorization token.
                AssertMatching(TestData.English.Weather.Utterance, await helper.GetFirstRecognizerResult(speechRecognizer));
            }
        }
    }
}