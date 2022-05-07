﻿// <copyright file="Resources.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2022 Chris Trout
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
 *     Runtime Version: .NET 6.0
 *
 *     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
 * </auto-generated>
 *---------------------------------------------------------------------------------------------------------*/

#pragma warning disable CA1055, CA1702, CA1707, CA1708 // This is T4-generated code from a resources file.

namespace MyTrout.Pipelines.Steps.Azure.Blobs
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
		/// Looks up a localized string like "Blob '{0}' already exists. in this container '{1}'.".
		/// </summary>
		public static string BLOB_ALREADY_EXISTS()
		{
			return Resources.BLOB_ALREADY_EXISTS(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' already exists. in this container '{1}'.".
		/// </summary>
		public static string BLOB_ALREADY_EXISTS(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("BLOB_ALREADY_EXISTS", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' already exists. in this container '{1}'.".
		/// </summary>
		public static string BLOB_ALREADY_EXISTS(params object[] args)
		{
			return Resources.BLOB_ALREADY_EXISTS(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' already exists. in this container '{1}'.".
		/// </summary>
		public static string BLOB_ALREADY_EXISTS(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("BLOB_ALREADY_EXISTS", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "Blob '{0}' does not exist in this container '{1}'.".
		/// </summary>
		public static string BLOB_DOES_NOT_EXIST()
		{
			return Resources.BLOB_DOES_NOT_EXIST(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' does not exist in this container '{1}'.".
		/// </summary>
		public static string BLOB_DOES_NOT_EXIST(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("BLOB_DOES_NOT_EXIST", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' does not exist in this container '{1}'.".
		/// </summary>
		public static string BLOB_DOES_NOT_EXIST(params object[] args)
		{
			return Resources.BLOB_DOES_NOT_EXIST(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Blob '{0}' does not exist in this container '{1}'.".
		/// </summary>
		public static string BLOB_DOES_NOT_EXIST(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("BLOB_DOES_NOT_EXIST", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "Container '{0}' does not exist.".
		/// </summary>
		public static string CONTAINER_DOES_NOT_EXIST()
		{
			return Resources.CONTAINER_DOES_NOT_EXIST(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Container '{0}' does not exist.".
		/// </summary>
		public static string CONTAINER_DOES_NOT_EXIST(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("CONTAINER_DOES_NOT_EXIST", culture);
		}
				
		/// <summary>
		/// Looks up a localized string using a specific culture like "Container '{0}' does not exist.".
		/// </summary>
		public static string CONTAINER_DOES_NOT_EXIST(params object[] args)
		{
			return Resources.CONTAINER_DOES_NOT_EXIST(CultureInfo.CurrentCulture, args);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Container '{0}' does not exist.".
		/// </summary>
		public static string CONTAINER_DOES_NOT_EXIST(CultureInfo culture, params object[] args)
		{
			return string.Format(culture, Resources.ResourceManager.GetString("CONTAINER_DOES_NOT_EXIST", culture), args);
		}

		/// <summary>
		/// Looks up a localized string like "Smoke Test (en-US)".
		/// </summary>
		public static string _SMOKE_TEST()
		{
			return Resources._SMOKE_TEST(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Looks up a localized string using a specific culture like "Smoke Test (en-US)".
		/// </summary>
		public static string _SMOKE_TEST(CultureInfo culture)
		{
			return Resources.ResourceManager.GetString("_SMOKE_TEST", culture);
		}
		
	}
}
#pragma warning restore CA1055, CA1702, CA1707, CA1708
