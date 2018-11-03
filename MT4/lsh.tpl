<chart>
id=131853577584890335
comment=53:51
symbol=GBPUSD
period=60
leftpos=10143
digits=5
scale=8
graph=1
fore=0
grid=0
volume=0
scroll=1
shift=1
ohlc=1
one_click=0
one_click_btn=1
askline=0
days=1
descriptions=0
shift_size=20
fixed_pos=0
window_left=64
window_top=64
window_right=1254
window_bottom=422
window_type=3
background_color=0
foreground_color=16777215
barup_color=65280
bardown_color=65280
bullcandle_color=0
bearcandle_color=16777215
chartline_color=65280
volumes_color=3329330
grid_color=10061943
askline_color=255
stops_color=255

<window>
height=100
fixed_height=0
<indicator>
name=main
<object>
type=0
object_name=Vertical Line 38753
period_flags=0
create_time=1540921185
color=255
style=0
weight=1
background=0
filling=0
selectable=1
hidden=0
zorder=0
time_0=1535558400
</object>
</indicator>
<indicator>
name=Bollinger Bands
period=20
shift=0
deviations=2.000000
apply=0
color=7451452
style=0
weight=1
period_flags=0
show_data=1
</indicator>
<indicator>
name=Moving Average
period=5
shift=0
method=0
apply=0
color=2139610
style=0
weight=1
period_flags=0
show_data=1
</indicator>
<indicator>
name=Moving Average
period=144
shift=0
method=0
apply=0
color=16711680
style=0
weight=2
period_flags=0
show_data=1
</indicator>
<indicator>
name=Custom Indicator
<expert>
name=AMA
flags=275
window_num=0
<inputs>
AMAPeriod=10
AMASlowest=60.0
AMAFastest=2.0
</inputs>
</expert>
shift_0=0
draw_0=0
color_0=65535
style_0=0
weight_0=0
period_flags=0
show_data=1
</indicator>
</window>

<window>
height=50
fixed_height=0
<indicator>
name=MACD
fast_ema=12
slow_ema=26
macd_sma=9
apply=0
color=12632256
style=0
weight=2
signal_color=255
signal_style=0
signal_weight=2
period_flags=0
show_data=1
</indicator>
<indicator>
name=Moving Average
period=1
shift=0
method=0
apply=7
color=16711680
style=0
weight=2
period_flags=0
show_data=1
</indicator>
</window>

<window>
height=50
fixed_height=0
<indicator>
name=Stochastic Oscillator
kperiod=8
dperiod=3
slowing=5
method=3
apply=0
color=11186720
style=0
weight=1
color2=255
style2=2
weight2=1
min=0.00000000
max=100.00000000
levels_color=12632256
levels_style=2
levels_weight=1
level_0=20.00000000
level_1=80.00000000
level_2=50.00000000
period_flags=0
show_data=1
</indicator>
</window>

<expert>
name=KClock
flags=275
window_num=0
</expert>
</chart>

