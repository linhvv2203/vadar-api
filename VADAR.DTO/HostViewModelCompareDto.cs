// <copyright file="HostViewModelCompareDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Host View Model Dto.
    /// </summary>
    public class HostViewModelCompareDto : IEqualityComparer<HostViewModelDto>
    {
        /// <summary>
        /// Products are equal if their names and product numbers are equal.
        /// Equalization method for 2 objects.
        /// </summary>
        /// <param name="x">First Host View Model Dto.</param>
        /// <param name="y">Second Host View Model Dto.</param>
        /// <returns>true: equal; false: difference.</returns>
        public bool Equals(HostViewModelDto x, HostViewModelDto y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            // Check whether the products' properties are equal.
            return x.Id == y.Id && x.Name == y.Name;
        }

        /// <summary>
        /// If Equals() returns true for a pair of objects
        /// then GetHashCode() must return the same value for these objects.
        /// </summary>
        /// <param name="product">Product.</param>
        /// <returns>int value.</returns>
        public int GetHashCode(HostViewModelDto product)
        {
            // Check whether the object is null
            if (ReferenceEquals(product, null))
            {
                return 0;
            }

            // Get hash code for the Name field if it is not null.
            var hashProductName = product.Name == null ? 0 : product.Name.GetHashCode();

            // Get hash code for the Code field.
            var hashProductCode = product.Id.GetHashCode();

            // Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }
}
