# Unsupported Features

Due to implementation difference in .NET, following Ecmascript features are not supported.

## Date

1. Date.prototype.setFullYear(0) not supported as .NET DateTime does not support setting year to 0. It will set to an Invalid Date.
2. Currently, Intl.DateTimeFormat of ECMA script is not supported. However, you can use .Net String Formats.

## Single Threaded Generators

1. Since CLR does not allow resumable functions, it is not possible to create generators without changing whole expression tree. This may be created in future.

## Number

1. Due to difference in double precision (15 digits), double values differ compared to V8 and Firefox. 

## Returning control from finally

1. .Net Lambda Compiler currently does not allow returning control from finally block. This is probably a minor fix, we can rewrite finally in non .net style to make it work.