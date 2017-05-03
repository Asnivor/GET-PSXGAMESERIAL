using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BizHawk.Common;
using BizHawk.Emulation;
using BizHawk.Emulation.DiscSystem;

namespace PSX_GetSerial
{
    public class MedDiscUtils
    {
        /// <summary>
        /// returns the PSX serial - Bizhawk DiscSystem requires either cue, ccd or iso (not bin or img)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPSXSerial(string path)
        {
            //path = @"G:\_Emulation\PSX\iso\Metal Gear Solid - Integral (J) [SLPM-86247]\Metal Gear Solid - Integral (J) (Disc 1) [SLPM-86247].cue";

            int lba = 23;
            Disc disc = Disc.LoadAutomagic(path);

            if (disc == null)
            {
                // unable to mount disc - return null
                return null;
            }

            var discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            var iso = new ISOFile();
            bool isIso = iso.Parse(new DiscStream(disc, discView, 0));

            if (isIso)
            {
                var appId = System.Text.Encoding.ASCII.GetString(iso.VolumeDescriptors[0].ApplicationIdentifier).TrimEnd('\0', ' ');

                var desc = iso.Root.Children;

                ISONode ifn = null;

                foreach (var i in desc)
                {
                    if (i.Key.Contains("SYSTEM.CNF"))
                        ifn = i.Value;
                }

                if (ifn == null)
                {
                    lba = 23;
                }
                else
                {
                    lba = Convert.ToInt32(ifn.Offset);
                }
            }
            else
            {
                lba = 23;
            }


            DiscIdentifier di = new DiscIdentifier(disc);

            // start by checking sector 23 (as most discs seem to have system.cfg there
            byte[] data = di.GetPSXSerialNumber(lba);
            // take first 32 bytes
            byte[] data32 = data.ToList().Take(46).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            if (!sS.Contains("cdrom:"))
            {
                return null;
            }

            // get the actual serial number from the returned string
            string[] arr = sS.Split(new string[] { "cdrom:" }, StringSplitOptions.None);
            string[] arr2 = arr[1].Split(new string[] { ";1" }, StringSplitOptions.None);
            string serial = arr2[0].Replace("_", "-").Replace(".", "");
            if (serial.Contains("\\"))
                serial = serial.Split('\\').Last();
            else
                serial = serial.TrimStart('\\').TrimStart('\\');

            // try and remove any nonsense after the serial
            string[] sarr2 = serial.Split('\r');
            if (sarr2.Length > 1)
                serial = sarr2.First();

            return serial;
        }

        
    }
}

