﻿namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides a unified way of converting member picker properties to strong typed model.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of the object to return.
    /// </typeparam>
    public class MemberPickerConverter<T> : TypeConverter where T : class
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(int) || typeof(IPublishedContent).IsAssignableFrom(sourceType))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            var content = value as IPublishedContent;
            if (content != null)
            {
                return content.As<T>();
            }

            if (value is int)
            {
                return this.ConvertFromInt((int)value);
            }

            int id;
            var s = value as string;
            if (s != null && int.TryParse(s, out id))
            {
                return this.ConvertFromInt(id);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Takes a member node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The member node ID.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        private object ConvertFromInt(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var umbracoHelper = ConverterHelper.UmbracoHelper;
            return umbracoHelper.TypedMember(id).As<T>();
        }
    }
}