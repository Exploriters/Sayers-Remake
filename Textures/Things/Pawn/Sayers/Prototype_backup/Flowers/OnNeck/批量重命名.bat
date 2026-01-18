@echo 正在批量重命名xwx！请确保此程序和目标文件在同一文件夹内！
@echo #注意：修改会自动提供_south、_north、_east、_west（若有），以及.png后缀名！所以无需输入它们。#
@echo #注意：可以创造遮罩文件哦！但其内容需要自己绘制，只是给个文件名。#
:start
@echo ======设置重命名======
@set /p current=请输入当前文件名:
@set /p target=请输入目标文件名:
@set mask=0
@set /p mask=是否创建遮罩文件(输入1则为是):
@set maskTip=否
@if %mask% neq 1 (set mask=0)
@if %mask%==1 (set maskTip=是)
@echo ======！预备！======
@echo 当前文件名:%current%(_south/_north/_east/_west).png
@echo 目标文件名:%target%(_south/_north/_east/_west).png
@echo 是否创建遮罩文件:%maskTip%
@echo ======重命名中……======
@set dir=_south
@set dirID=0
:loop
@if %dirID%==0 (set dir=_south)
@if %dirID%==1 (set dir=_north)
@if %dirID%==2 (set dir=_east)
@if %dirID%==3 (set dir=_west)
@if exist %current%%dir%.png (
  if exist %current%%dir%m.png (
    ren "%current%%dir%m.png" "%target%%dir%m.png"
    echo 已修改"%current%%dir%m.png"为"%target%%dir%m.png"！（遮罩）
  ) else (
    if %mask%==1 (
      copy "%current%%dir%.png" "%target%%dir%m.png"
      echo 已创建"%target%%dir%m.png"！（遮罩）
	)
  )
  ren "%current%%dir%.png" "%target%%dir%.png"
  echo 已修改"%current%%dir%.png"为"%target%%dir%.png"！
) else (
  echo 找不到"%current%%dir%.png"！
)
@if %dirID%==3 (goto loopEnd)
@set /a dirID+=1
@goto loop
:loopEnd
@echo ======！完毕！======
@set restart=0
@set /p restart=是否继续重命名(输入1则为是):
@if %restart%==1 (
	goto start
)
@pause