set %date% = date /t

ApiNormalizer.exe C:\Work\OmsApi2\%date%

pushd C:\Work\OmsApi2
copy %date%\*.json .
git add .
git commit -m %date%
popd

pause