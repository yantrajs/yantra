# Unsupported Features

Due to implementation difference in .NET, following Ecmascript features are not supported.

## Date

1. Date.prototype.setFullYear(0) not supported as .NET DateTime does not support setting year to 0. 