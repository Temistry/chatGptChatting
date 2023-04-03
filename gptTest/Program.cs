using Microsoft.Extensions.Configuration;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WinFormsChatGPT;
//using System.Windows.Forms;

namespace WinFormsChatGPT
{
    public partial class gptInterface
    {
        IConfiguration configuration;
        string apiKey;

        public gptInterface()
        {
            apiKey = ""; // key 넣기;
        }

        public void Update()
        {
            RequestGPT();
        }

        // chatGpt.Completions.CreateCompletion(new CompletionCreateRequest() request and wait function



        async Task RequestGPT()
        {
            var chatGpt = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });

            WrapAPI.SetConsoleTextColor(ConTextColor.LIGHT_WHITE);
            Console.Write("나 : ");
            string prompote = Console.ReadLine();
            Log(prompote + "\n");
            Console.WriteLine("답변을 기다리는중");

            var completionResult = await chatGpt.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = prompote,
                Model = Models.TextDavinciV3, //모델명
                Temperature = 0.9F,      //대답의 자유도(다양성 - Diversity)). 자유도가 낮으면 같은 대답, 높으면 좀 아무말?
                MaxTokens = 3000,      //이게 길수록 글자가 많아짐. 짧은 답장은 상관없으나 이게 100,200으로 짧으면 말을 짤라버림 (시간제약이 있거나 썸네일식으로 확인만 할때는 낮추면 좋을 듯. 추가로 토큰은 1개 단어라고 생각하면 편한데, 정확하게 1개 단어는 아닌 (1개 단어가 될수도 있고 긴단어는 2개 단어가 될수 있음. GPT 검색의 단위가된다고 함. 이 토큰 단위를 기준으로 트래픽이 매겨지고, (유료인경우) 과금 책정이 됨)
                N = 1   //경우의 수(대답의 수). N=3으로 하면 3번 다른 회신을 배열에 담아줌
            });

            WrapAPI.SetConsoleTextColor(ConTextColor.LIGHT_GREEN);

            if (completionResult.Successful)
            {
                foreach (var choice in completionResult.Choices)
                {
                    string answer = "\n" + "GPT : " + choice.Text;
                    Console.WriteLine(answer);
                    Log(answer);
                }
                Console.WriteLine();
                Log("\n");
            }
            else
            {
                if (completionResult.Error == null)
                {
                    Log("Unknown Error");
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }
            RequestGPT();
        }
        public void Log(string str)
        {
            // 현재 위치 경로
            string currentDirectoryPath = Environment.CurrentDirectory.ToString();
            // Logs 디렉토리 경로(현재 경로에 Logs라는 디렉토리 경로 합치기)
            string DirPath = System.IO.Path.Combine(currentDirectoryPath, "Logs");
            // Logs\Log_yyyyMMdd.log 형식의 로그 파일 경로
            string FilePath = DirPath + @"\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            // Logs 디렉토리 정보
            DirectoryInfo di = new DirectoryInfo(DirPath);
            // 로그 파일 경로 정보
            FileInfo fi = new FileInfo(FilePath);
            try
            {
                // Logs 디렉토리가 없을 경우 생성
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                // 오류 메세지 생성
                string error_string = string.Format("{0}: \t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), str);

                // Log 파일이 존재할 경우와 존재하지 않을 경우로 나누어서 진행
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        sw.WriteLine(error_string);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        sw.WriteLine(error_string);
                        sw.Close();
                    }
                }
            }
            catch
            {
                // 사실 try 부분에서 오류가 나면 답이 없습니다.
                // 좋은 의견 있으시면 댓글로 남겨주시길 바랍니다.
            }
        }
    }

    public enum ConTextColor
    {
        LACK, BLUE, GREEN, JADE, RED,
        PURPLE, YELLOW, WHITE, GRAY, LIGHT_BLUE, LIGHT_GREEN,
        LIGHT_JADE, LIGHT_RED, LIGHT_PURPLE,
        LIGHT_YELLOW, LIGHT_WHITE
    };

    public static class WrapAPI
    {
        [DllImport("Kernel32.dll")]
        static extern int SetConsoleTextAttribute(IntPtr hConsoleOutput, short wAttributes);

        [DllImport("Kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        const int STD_OUTPUT_HANDLE = -11;
        public static void SetConsoleTextColor(ConTextColor color)
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            SetConsoleTextAttribute(handle, (short)color);
        }
    }

   
}



namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            gptInterface form = new gptInterface();
            while (true) { form.Update(); };
            
        }
    }
}