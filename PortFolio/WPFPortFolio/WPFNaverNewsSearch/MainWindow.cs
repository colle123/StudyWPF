using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFNaverNewsSearch.Helpers;

namespace WPFNaverNewsSearch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                //Commons.ShowMessageAsync("실행", "뉴스검색 실행!");
                SearchNaverNews();
            }
        }

        private void SearchNaverNews()
        {
            string keyword = txtSearch.Text;
            string clientID = "_Qjhnv3RpqvZSN06Qd3B";
            string clientSecret = "WzLF7rg4NU";
            string base_url = $"https://openapi.naver.com/v1/search/news.json?start={txtStartNum.Text}&display=10&query={keyword}";
            string result;

            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;

            // Naver OpenAPI 실제 요청
            try
            {
                request = WebRequest.Create(base_url);
                request.Headers.Add("X-Naver-Client-Id", clientID); // 중요!
                request.Headers.Add("X-Naver-Client-Secret", clientSecret); // 중요!!

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                stream.Close();
                response.Close();
            }

            var parseJson = JObject.Parse(result); // string to json

            int total = Convert.ToInt32(parseJson["total"]); // 전체 검색결과 수
            int display = Convert.ToInt32(parseJson["display"]);

            var items = parseJson["items"];
            var json_array = (JArray)items;

            List<NewsItem> newsItems = new List<NewsItem>(); // 데이터그리드와 연동하기 위한 List


            foreach (var item in json_array)
            {
                var temp = DateTime.Parse(item["pubDate"].ToString());
                NewsItem news = new NewsItem()
                {
                    Title = item["title"].ToString(),
                    OriginalLink = item["originallink"].ToString(),
                    Link = item["link"].ToString(),
                    Description = item["description"].ToString(),
                    PubDate = temp.ToString("yyyy-MM-dd HH:mm")              
                };

                newsItems.Add(news);
            }

            this.DataContext = newsItems;

            stsResult.Content = $"{total}개 중 {display}개 호출 성공!";
        }

        private void dgrResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgrResult.SelectedItem == null) return; // 두 번째 검색부터 나는 null 오류를 제거해줌

            string link = (dgrResult.SelectedItem as NewsItem).Link;
            Process.Start(link);
            
        }

        private void txtStartNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Commons.ShowMessageAsync("실행", "뉴스검색 실행!");
                SearchNaverNews();
            }
        }
    }

    internal class NewsItem // 다른 cs파일을 만들어서 Class를 만들때는 Volume이 큰 Class만 해당, 작으면 같은 cs안에 만들어도 됨(또는 구조체)
                            // Class는 명사와 동사의 집합. 멤버변수는 모두 명사, 멤버함수는 동사
    {
        public string Title { get; set; }
        public string OriginalLink { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}
