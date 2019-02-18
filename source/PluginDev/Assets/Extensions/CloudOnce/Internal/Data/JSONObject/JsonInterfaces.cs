// <copyright file="JsonInterfaces.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.Internal
{
    public interface IJsonConvertible : IJsonSerializable, IJsonDeserializable
    {
    }

    public interface IJsonSerializable
    {
        JSONObject ToJSONObject();
    }

    public interface IJsonDeserializable
    {
        void FromJSONObject(JSONObject jsonObject);
    }
}
