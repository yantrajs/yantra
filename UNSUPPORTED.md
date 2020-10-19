# Unsupported Features

Due to implementation difference in .NET, following Ecmascript features are not supported.

## Date

1. Date.prototype.setFullYear(0) not supported as .NET DateTime does not support setting year to 0. It will set to an Invalid Date.
2. Currently, Intl.DateTimeFormat of ECMA script is not supported. However, you can use .Net String Formats.