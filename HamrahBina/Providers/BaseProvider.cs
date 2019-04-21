using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Providers
{
    /// <summary>
    /// Base interface which should be implemented by every and each OCR api which is used.
    /// </summary>
    public interface IOCRProvider
    {
        /// <summary>
        /// Base method for interacting with OCR apis
        /// </summary>
        /// <returns></returns>
        string Transform();
    }

    /// <summary>
    /// A dummy class which returns a constant text.
    /// It is used for accelerating development process.
    /// </summary>
    public class DummyProvider : IOCRProvider
    {
        public string Transform()
        {
            return "This is a dummy text";
        }
    }
}