ECHO off

cls

SET packagePath="UniSpreadsheets/Assets/UniSpreadsheets"
SET branch=upm

SET /p version="Enter version: "

git subtree split --prefix=%packagePath% --branch %branch%
ECHO.
git tag %version% %branch%
ECHO.
git push origin %branch% --tags
ECHO.

PAUSE