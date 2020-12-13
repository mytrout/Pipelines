﻿// <copyright file="Resources.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
namespace MyTrout.Pipelines.Steps.Data
{
	using System.Globalization;

	/// <summary>
	/// The <see cref="Resources" /> class provides a wrapper around resource files that allows localized strings to be returned, as needed.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
		/// Looks up a localized string like "No records were returned from the database during the execution of '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NO_DATA_FOUND()
		{
			return Resources.NO_DATA_FOUND(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "No records were returned from the database during the execution of '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NO_DATA_FOUND(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("NO_DATA_FOUND", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "No records were returned from the database during the execution of '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NO_DATA_FOUND(params object[] args)
		{
			return Resources.NO_DATA_FOUND(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "No records were returned from the database during the execution of '{0}'.".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is T4-generated code.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is T4-generated code from a resources file.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "This is T4-generated code from a resources file.")]
		public static string NO_DATA_FOUND(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("NO_DATA_FOUND", culture), args);
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