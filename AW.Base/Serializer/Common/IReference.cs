﻿namespace AW
{
    public interface IReference
    {
        /// <summary>
        /// Used by the serializer for reference types
        /// </summary>
        int ReferenceId { get; set; }
    }
}
