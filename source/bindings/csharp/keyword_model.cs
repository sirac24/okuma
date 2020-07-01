//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.

using System;
using Microsoft.CognitiveServices.Speech.Internal;
using static Microsoft.CognitiveServices.Speech.Internal.SpxExceptionThrower;

namespace Microsoft.CognitiveServices.Speech
{
    /// <summary>
    /// Represents keyword recognition model used with StartKeywordRecognitionAsync.
    /// </summary>
    /// Note: Keyword spotting (KWS) functionality might work with any microphone type, official KWS support, however, is currently limited to the microphone arrays found in the Azure Kinect DK hardware or the Speech Devices SDK.
    public sealed class KeywordRecognitionModel : IDisposable
    {
        /// <summary>
        /// Creates a keyword recognition model using the specified file.
        /// </summary>
        /// <param name="fileName">A string that represents file name for the keyword recognition model.</param>
        /// <returns>The keyword recognition model being created.</returns>
        public static KeywordRecognitionModel FromFile(string fileName)
        {
            IntPtr keywordModelHandle = IntPtr.Zero;
            ThrowIfFail(Internal.KeywordRecognitionModel.keyword_recognition_model_create_from_file(fileName, out keywordModelHandle));
            return new KeywordRecognitionModel(keywordModelHandle);
        }

        /// <summary>
        /// Dispose of associated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed resources
                keywordHandle?.Dispose();
            }
            // dispose unmanaged resources
            disposed = true;
        }

        private KeywordRecognitionModel(IntPtr keywordHandlePtr)
        {
            keywordHandle = new InteropSafeHandle(keywordHandlePtr, Internal.KeywordRecognitionModel.keyword_recognition_model_handle_release);
        }

        private bool disposed = false;
        internal InteropSafeHandle keywordHandle;
    }
}
