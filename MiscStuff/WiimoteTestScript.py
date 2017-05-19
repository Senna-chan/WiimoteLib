def log_motionplus():
   diagnostics.watch(wiimote[0].ahrs.yaw)
   diagnostics.watch(wiimote[0].ahrs.pitch)
   diagnostics.watch(wiimote[0].ahrs.roll)
   
def simulate_mouse_pointer():   
   mouse.deltaX = filters.deadband(filters.delta(wiimote[0].ahrs.yaw), 0.1) * 10
   mouse.deltaY = filters.deadband(-filters.delta(wiimote[0].ahrs.pitch), 0.1) * 10
   
def log_nunchuck():
   diagnostics.watch(wiimote[0].nunchuck.acceleration.x)
   diagnostics.watch(wiimote[0].nunchuck.acceleration.y)
   diagnostics.watch(wiimote[0].nunchuck.acceleration.z)
   diagnostics.watch(wiimote[0].nunchuck.buttons.button_down(NunchuckButtons.Z))

def simulate_mouse_press():
   mouse.leftButton = wiimote[0].nunchuck.buttons.button_down(NunchuckButtons.Z)

if starting:
   system.setThreadTiming(TimingTypes.HighresSystemTimer)
   system.threadExecutionInterval = 2
   
   wiimote[0].motionplus.update += log_motionplus
   wiimote[0].motionplus.update += simulate_mouse_pointer
   wiimote[0].nunchuck.update += log_nunchuck
   wiimote[0].nunchuck.update += simulate_mouse_press
   
   wiimote[0].enable(WiimoteCapabilities.MotionPlus | WiimoteCapabilities.Extension)