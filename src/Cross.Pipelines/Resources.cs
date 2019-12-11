﻿// <copyright file="Resources.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
/*-----------------------------------------------------------------------------------------------------------
 * <auto-generated>
 *     This code was generated by a tool.
 *     Runtime Version: .NET Standard 2.0
 *
 *     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
 * </auto-generated>
 *---------------------------------------------------------------------------------------------------------*/
namespace Cross.Pipelines
{
	using System.Globalization;

	/// <summary>
	/// The <see cref="Resources" /> class provides a wrapper around resource files that allows localized strings to be returned, as needed.
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// Initializes static properties of the <see cref="Resources" /> class.
		/// </summary>
		static Resources()
		{
			Resources.ResourceManager = new global::System.Resources.ResourceManager(typeof(Resources));
		}

		/// <summary>
		/// Gets a singleton <see cref="System.Resources.ResourceManager" /> instance of the local resource manager.
		/// </summary>
		public static global::System.Resources.ResourceManager ResourceManager { get; private set; }

		/// <summary>
		/// Looks up a localized string like "'{0}' middleware does not contain a constructor that has a PipelineRequest parameter.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER()
		{
			return Resources.CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain a constructor that has a PipelineRequest parameter.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain a constructor that has a PipelineRequest parameter.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(params object[] args)
		{
			return Resources.CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain a constructor that has a PipelineRequest parameter.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "PipelineContext cannot be constructed without a null, empty, or whitespace PipelineName.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONTEXT_CANNOT_BE_CONSTRUCTED()
		{
			return Resources.CONTEXT_CANNOT_BE_CONSTRUCTED(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "PipelineContext cannot be constructed without a null, empty, or whitespace PipelineName.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string CONTEXT_CANNOT_BE_CONSTRUCTED(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("CONTEXT_CANNOT_BE_CONSTRUCTED", culture);
		}
		
		/// <summary>
		/// Looks up a localized string like "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_DOES_NOT_EXIST()
		{
			return Resources.INVOKEASYNC_DOES_NOT_EXIST(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_DOES_NOT_EXIST(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("INVOKEASYNC_DOES_NOT_EXIST", culture);
		}
		
		/// <summary>
		/// Looks up a localized string like "'{0}' middleware does not contain the '{1}' method."".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_MISSING()
		{
			return Resources.INVOKEASYNC_MISSING(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain the '{1}' method."".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_MISSING(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("INVOKEASYNC_MISSING", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain the '{1}' method."".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_MISSING(params object[] args)
		{
			return Resources.INVOKEASYNC_MISSING(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "'{0}' middleware does not contain the '{1}' method."".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string INVOKEASYNC_MISSING(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("INVOKEASYNC_MISSING", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "A 'null' middleware was added to the Pipeline which prevents the pipeline from being built as requested.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NULL_MIDDLEWARE()
		{
			return Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "A 'null' middleware was added to the Pipeline which prevents the pipeline from being built as requested.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NULL_MIDDLEWARE(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("NULL_MIDDLEWARE", culture);
		}
		
		/// <summary>
		/// Looks up a localized string like "ServiceProvider does not contain one of the parameters required for '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string SERVICEPROVIDER_LACKS_PARAMETER()
		{
			return Resources.SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "ServiceProvider does not contain one of the parameters required for '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("SERVICEPROVIDER_LACKS_PARAMETER", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "ServiceProvider does not contain one of the parameters required for '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string SERVICEPROVIDER_LACKS_PARAMETER(params object[] args)
		{
			return Resources.SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "ServiceProvider does not contain one of the parameters required for '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("SERVICEPROVIDER_LACKS_PARAMETER", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "{0} must be a reference type.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string TYPE_MUST_BE_REFERENCE()
		{
			return Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "{0} must be a reference type.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string TYPE_MUST_BE_REFERENCE(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("TYPE_MUST_BE_REFERENCE", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "{0} must be a reference type.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string TYPE_MUST_BE_REFERENCE(params object[] args)
		{
			return Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "{0} must be a reference type.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string TYPE_MUST_BE_REFERENCE(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("TYPE_MUST_BE_REFERENCE", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "Smoke Test (en-US)".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string _SMOKE_TEST()
		{
			return Resources._SMOKE_TEST(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Smoke Test (en-US)".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string _SMOKE_TEST(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("_SMOKE_TEST", culture);
		}
		}
}
