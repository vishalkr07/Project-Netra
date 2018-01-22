using AudioSwitcher.AudioApi.CoreAudio;

namespace ProjectNetra
{
    public class Volume_Control                             // Class for controlling volume of the system
    {
        private static CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        public static void VolumeUp()
        {
            defaultPlaybackDevice.Volume += 2;
        }
        public static void VolumeDown()
        {
            defaultPlaybackDevice.Volume -= 2;
        }
        public static void Mute()
        {
            defaultPlaybackDevice.Mute(true);
        }
    }
}
