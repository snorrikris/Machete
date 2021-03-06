﻿namespace Machete
{
    public delegate Value<T> SetValueProvider<TInput, TSchema, T>(TranslateValueContext<T, TInput, TSchema> context)
        where TSchema : Entity;


    public delegate T SetValueProvider<out T>(TextSlice slice);
}