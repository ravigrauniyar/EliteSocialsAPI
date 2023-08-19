﻿using System.Text.Json.Serialization;

namespace Application.ApiResponseService
{
    public class ApiResponseData<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T value { get; set; } = default;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<T> itemList { get; set; } = default;
    }
}
