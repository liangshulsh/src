using System;
using System.ServiceProcess;
using System.Text;

namespace LaunchProcessFromService
{
    static class Program
    {


        static void Main(string[] args)
        {
            string pwd = "ShuliA3342!@#$%^&";
            string encrypt = encrypt1(pwd);
            Console.WriteLine(encrypt);
            encrypt = encrypt1(pwd);
            Console.WriteLine(encrypt);
            string origial = decrpyt1(encrypt);
            Console.Write(origial);
            ProcessLauncher service = new ProcessLauncher();
            if (args.Length > 0 && args[0] == "/i")
            {
                //Launched not as service.
                service.LaunchCommandLine();
            }
            else
            {
                //Standard service entry point.
                ServiceBase.Run(service);
            }
        }

        static string encrypt0(string orignal)
        {
            string[] samples = new string[2];
            samples[0] = "=-+_0987P654321`)(*&^Q%$#A@!~gZhUfjJXdkSsMlaK;'I:O\"WqEwerDtyLuiCop{V}F|[]R\\T?/>.G<,zBcNbmHnvYx";
            samples[1] = "1qaQz!E@2WwsxTcdeR3#4r$fYvbgtU5%hIn^Oy6&Pj7uAm<S,Di*F8kG9(HolJ.>/K?;L:p0)Z-X['C\"V{_B+N]M=}\\|~`";

            StringBuilder result = new StringBuilder();
            for (int i = orignal.Length - 1; i >= 0; i--)
            {
                result.Append(samples[1].IndexOf(orignal[i]) >= 0 ? samples[0][samples[1].IndexOf(orignal[i])] : orignal[i]);
            }

            return result.ToString();
        }

        static string decrypt0(string dest)
        {
            string[] samples = new string[2];
            samples[0] = "=-+_0987P654321`)(*&^Q%$#A@!~gZhUfjJXdkSsMlaK;'I:O\"WqEwerDtyLuiCop{V}F|[]R\\T?/>.G<,zBcNbmHnvYx";
            samples[1] = "1qaQz!E@2WwsxTcdeR3#4r$fYvbgtU5%hIn^Oy6&Pj7uAm<S,Di*F8kG9(HolJ.>/K?;L:p0)Z-X['C\"V{_B+N]M=}\\|~`";

            StringBuilder result = new StringBuilder();
            for (int i = dest.Length - 1; i >= 0; i--)
            {
                result.Append(samples[0].IndexOf(dest[i]) >= 0 ? samples[1][samples[0].IndexOf(dest[i])] : dest[i]);
            }

            return result.ToString();
        }

        static string encrypt1(string orignal)
        {
            string[] samples = new string[2];
            samples[0] = "=-+_0987P654321`)(*&^Q%$#A@!~gZhUfjJXdkSsMlaK;'I:O\"WqEwerDtyLuiCop{V}F|[]R\\T?/>.G<,zBcNbmHnvYx";
            samples[1] = "1qaQz!E@2WwsxTcdeR3#4r$fYvbgtU5%hIn^Oy6&Pj7uAm<S,Di*F8kG9(HolJ.>/K?;L:p0)Z-X['C\"V{_B+N]M=}\\|~`";

            orignal = encrypt0(orignal);

            string sampleString = samples[1];

            StringBuilder result = new StringBuilder();

            Random rand = new Random(orignal.Length + orignal.GetHashCode() / 10000 + sampleString.GetHashCode());
            
            for (int i = 0; i < orignal.Length; i++)
            {
                result.Append(sampleString.IndexOf(orignal[i]) >= 0 ? sampleString[(sampleString.IndexOf(orignal[i])+i+1) % sampleString.Length] : orignal[i]);
                int num = Convert.ToInt32(orignal[i]) * Convert.ToInt32(orignal[i]) * 27 % sampleString.Length;
                for (int j = 0; j < num; j++)
                {
                    result.Append(sampleString[rand.Next(sampleString.Length - 1)]);
                }
            }

            return decrypt0(result.ToString());
        }

        static string decrpyt1(string dest)
        {
            string[] samples = new string[2];
            samples[0] = "=-+_0987P654321`)(*&^Q%$#A@!~gZhUfjJXdkSsMlaK;'I:O\"WqEwerDtyLuiCop{V}F|[]R\\T?/>.G<,zBcNbmHnvYx";
            samples[1] = "1qaQz!E@2WwsxTcdeR3#4r$fYvbgtU5%hIn^Oy6&Pj7uAm<S,Di*F8kG9(HolJ.>/K?;L:p0)Z-X['C\"V{_B+N]M=}\\|~`";

            string sampleString = samples[1];

            dest = encrypt0(dest);

            StringBuilder result = new StringBuilder();

            int retCount = 1;
            int i = 0;
            while (i < dest.Length)
            {
                char c = sampleString.IndexOf(dest[i]) >= 0 ? sampleString[(sampleString.IndexOf(dest[i]) - retCount + retCount * sampleString.Length) % sampleString.Length] : dest[i];
                result.Append(c);
                retCount++;
                i++;
                i += Convert.ToInt32(c) * Convert.ToInt32(c) * 27 % sampleString.Length;
            }

            return decrypt0(result.ToString());
        }
    }


}
