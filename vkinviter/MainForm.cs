using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace vkinviter
{
    public partial class MainForm : Form
    {
        private static string _bwStatus;
        BackgroundWorker backgroundWorker1;
        public string CityName;
        public string CountryId;
        public Dictionary<string, int> Countries;
        Inviter vkInv;
        ResultForm resultForm;
 
        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();

            resultForm = new ResultForm();
            
            textBoxGroupLink.Text = @"http://vk.com/v7paca";
            textBoxEventLink.Text = @"http://vk.com/7pacateatr";

            //var cityFilterSelData = {"countries":[[1,"<b>Россия<\/b>"],[2,"Украина"],[3,"Беларусь"],[4,"Казахстан"],[5,"Азербайджан"],[6,"Армения"],[7,"Грузия"],[8,"Израиль"],[9,"США"],[65,"Германия"],[11,"Кыргызстан"],[12,"Латвия"],[13,"Литва"],[14,"Эстония"],[15,"Молдова"],[16,"Таджикистан"],[17,"Туркменистан"],[18,"Узбекистан"],[19,"Австралия"],[20,"Австрия"],[21,"Албания"],[22,"Алжир"],[23,"Американское Самоа"],[24,"Ангилья"],[25,"Ангола"],[26,"Андорра"],[27,"Антигуа и Барбуда"],[28,"Аргентина"],[29,"Аруба"],[30,"Афганистан"],[31,"Багамы"],[32,"Бангладеш"],[33,"Барбадос"],[34,"Бахрейн"],[35,"Белиз"],[36,"Бельгия"],[37,"Бенин"],[38,"Бермуды"],[39,"Болгария"],[40,"Боливия"],[235,"Бонайре, Синт-Эстатиус и Саба"],[41,"Босния и Герцеговина"],[42,"Ботсвана"],[43,"Бразилия"],[44,"Бруней-Даруссалам"],[45,"Буркина-Фасо"],[46,"Бурунди"],[47,"Бутан"],[48,"Вануату"],[233,"Ватикан"],[49,"Великобритания"],[50,"Венгрия"],[51,"Венесуэла"],[52,"Виргинские острова, Британские"],[53,"Виргинские острова, США"],[54,"Восточный Тимор"],[55,"Вьетнам"],[56,"Габон"],[57,"Гаити"],[58,"Гайана"],[59,"Гамбия"],[60,"Гана"],[61,"Гваделупа"],[62,"Гватемала"],[63,"Гвинея"],[64,"Гвинея-Бисау"],[66,"Гибралтар"],[67,"Гондурас"],[68,"Гонконг"],[69,"Гренада"],[70,"Гренландия"],[71,"Греция"],[72,"Гуам"],[73,"Дания"],[231,"Джибути"],[74,"Доминика"],[75,"Доминиканская Республика"],[76,"Египет"],[77,"Замбия"],[78,"Западная Сахара"],[79,"Зимбабве"],[80,"Индия"],[81,"Индонезия"],[82,"Иордания"],[83,"Ирак"],[84,"Иран"],[85,"Ирландия"],[86,"Исландия"],[87,"Испания"],[88,"Италия"],[89,"Йемен"],[90,"Кабо-Верде"],[91,"Камбоджа"],[92,"Камерун"],[10,"Канада"],[93,"Катар"],[94,"Кения"],[95,"Кипр"],[96,"Кирибати"],[97,"Китай"],[98,"Колумбия"],[99,"Коморы"],[100,"Конго"],[101,"Конго, демократическая республика"],[102,"Коста-Рика"],[103,"Кот д`Ивуар"],[104,"Куба"],[105,"Кувейт"],[138,"Кюрасао"],[106,"Лаос"],[107,"Лесото"],[108,"Либерия"],[109,"Ливан"],[110,"Ливия"],[111,"Лихтенштейн"],[112,"Люксембург"],[113,"Маврикий"],[114,"Мавритания"],[115,"Мадагаскар"],[116,"Макао"],[117,"Македония"],[118,"Малави"],[119,"Малайзия"],[120,"Мали"],[121,"Мальдивы"],[122,"Мальта"],[123,"Марокко"],[124,"Мартиника"],[125,"Маршалловы Острова"],[126,"Мексика"],[127,"Микронезия, федеративные штаты"],[128,"Мозамбик"],[129,"Монако"],[130,"Монголия"],[131,"Монтсеррат"],[132,"Мьянма"],[133,"Намибия"],[134,"Науру"],[135,"Непал"],[136,"Нигер"],[137,"Нигерия"],[139,"Нидерланды"],[140,"Никарагуа"],[141,"Ниуэ"],[142,"Новая Зеландия"],[143,"Новая Каледония"],[144,"Норвегия"],[145,"Объединенные Арабские Эмираты"],[146,"Оман"],[147,"Остров Мэн"],[148,"Остров Норфолк"],[149,"Острова Кайман"],[150,"Острова Кука"],[151,"Острова Теркс и Кайкос"],[152,"Пакистан"],[153,"Палау"],[154,"Палестинская автономия"],[155,"Панама"],[156,"Папуа - Новая Гвинея"],[157,"Парагвай"],[158,"Перу"],[159,"Питкерн"],[160,"Польша"],[161,"Португалия"],[162,"Пуэрто-Рико"],[163,"Реюньон"],[164,"Руанда"],[165,"Румыния"],[166,"Сальвадор"],[167,"Самоа"],[168,"Сан-Марино"],[169,"Сан-Томе и Принсипи"],[170,"Саудовская Аравия"],[171,"Свазиленд"],[172,"Святая Елена"],[173,"Северная Корея"],[174,"Северные Марианские острова"],[175,"Сейшелы"],[176,"Сенегал"],[177,"Сент-Винсент"],[178,"Сент-Китс и Невис"],[179,"Сент-Люсия"],[180,"Сент-Пьер и Микелон"],[181,"Сербия"],[182,"Сингапур"],[234,"Синт-Мартен"],[183,"Сирийская Арабская Республика"],[184,"Словакия"],[185,"Словения"],[186,"Соломоновы Острова"],[187,"Сомали"],[188,"Судан"],[189,"Суринам"],[190,"Сьерра-Леоне"],[191,"Таиланд"],[192,"Тайвань"],[193,"Танзания"],[194,"Того"],[195,"Токелау"],[196,"Тонга"],[197,"Тринидад и Тобаго"],[198,"Тувалу"],[199,"Тунис"],[200,"Турция"],[201,"Уганда"],[202,"Уоллис и Футуна"],[203,"Уругвай"],[204,"Фарерские острова"],[205,"Фиджи"],[206,"Филиппины"],[207,"Финляндия"],[208,"Фолклендские острова"],[209,"Франция"],[210,"Французская Гвиана"],[211,"Французская Полинезия"],[212,"Хорватия"],[213,"Центрально-Африканская Республика"],[214,"Чад"],[230,"Черногория"],[215,"Чехия"],[216,"Чили"],[217,"Швейцария"],[218,"Швеция"],[219,"Шпицберген и Ян Майен"],[220,"Шри-Ланка"],[221,"Эквадор"],[222,"Экваториальная Гвинея"],[223,"Эритрея"],[224,"Эфиопия"],[226,"Южная Корея"],[227,"Южно-Африканская Республика"],[232,"Южный Судан"],[228,"Ямайка"],[229,"Япония"]],"country":"","cities":[],"city":""};
            Countries = new Dictionary<string, int>();
            Countries.Add("Россия", 1);
            Countries.Add("Украина", 2);
            Countries.Add("Беларусь", 3);
            Countries.Add("Казахстан", 4);
            Countries.Add("Латвия", 12);
            Countries.Add("Литва", 13);
            Countries.Add("Эстония", 14);
            Countries.Add("Молдова", 15);

            comboBoxCountry.Items.AddRange(Countries.Keys.ToArray());
            comboBoxCountry.SelectedIndex = 0;

            List<string> cities = new List<string>();
            cities.Add("Москва");
            cities.Add("Санкт-Петербург");
            cities.Add("Москва, Балашиха, Химки, Подольск, Королев, Мытищи, Люберцы, Коломна, Одинцово, Долгопрудный, Реутов, Чехов");

            comboBoxCity.Items.AddRange(cities.ToArray());
            comboBoxCity.SelectedIndex = 0;
        }

        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1 = new BackgroundWorker();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }
       
        private void buttonStart_Click(object sender, EventArgs e)
        {
            Logger.LogMethod();

            Logger.AddText("textBoxLogin.Text == {0}", textBoxLogin.Text);
            Logger.AddText("textBoxGroupLink.Text == {0}", textBoxGroupLink.Text);
            Logger.AddText("textBoxEventLink.Text == {0}", textBoxEventLink.Text);
            Logger.AddText("comboBoxCountry.SelectedText == {0}", comboBoxCountry.SelectedItem.ToString());
            Logger.AddText("comboBoxCity.SelectedText == {0}", comboBoxCity.Text.ToString());

            
            string groupId = Inviter.GetGroupIdByUrl(textBoxGroupLink.Text);
            string eventId = Inviter.GetGroupIdByUrl(textBoxEventLink.Text);

            string countryId = Countries[comboBoxCountry.SelectedItem.ToString()].ToString();
            string citiesList = comboBoxCity.Text.ToString();

            Dictionary<string, string> workArgs = new Dictionary<string, string>();

            workArgs.Add("login", textBoxLogin.Text);
            workArgs.Add("password", textBoxPassword.Text);
            workArgs.Add("countryId", countryId);
            workArgs.Add("citiesList", citiesList);
            workArgs.Add("groupId", groupId);
            workArgs.Add("eventId", eventId);
            
            vkInv = new Inviter();
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync(workArgs);
            }
        }
        // This event handler is where the time-consuming work is done. 
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.LogMethod();

            BackgroundWorker worker = sender as BackgroundWorker;
            Dictionary<string, string> args = e.Argument as Dictionary<string, string>;

            vkInv.Login = args["login"];
            vkInv.Password = args["password"];
            string countryId = args["countryId"];
            string citiesList = args["citiesList"];
            string groupId = args["groupId"];
            string eventId = args["eventId"];

            _bwStatus = "Logon";
            worker.ReportProgress(0);
            vkInv.Logon();

            _bwStatus = "Get cities list";
            worker.ReportProgress(0);
            List<string> listCitiesId = VkCityEx.GetListOfCityIdByNames(countryId, citiesList, worker);
            if ((listCitiesId == null) || (listCitiesId.Count == 0))
                throw new ArgumentNullException();

            StatisticsAndReport.CityNames = citiesList;

            _bwStatus = "Get all users in the cities";
            worker.ReportProgress(0);
            List<VkUser> usersInTheCity = new List<VkUser>();
            foreach(string cityId in listCitiesId)
            {
                usersInTheCity.AddRange(vkInv.GetAllUsersInCity(groupId, countryId, cityId, worker));
            }
            if (usersInTheCity.Count == 0)
                throw new ArgumentNullException();
            StatisticsAndReport.UserCountInCity = usersInTheCity.Count;

            _bwStatus = "Get all group member for inventation";
            worker.ReportProgress(0);
            List<VkUser> friends = vkInv.GetFriendsListForInventation(eventId, worker);
            if ((friends == null) || (friends.Count == 0))
                throw new ArgumentNullException();

            usersInTheCity = usersInTheCity.GetHasIdFrom(friends);

            _bwStatus = "Sending invites to users";
            worker.ReportProgress(0);
            vkInv.InviteCycle(eventId, usersInTheCity, worker);
            worker.ReportProgress(100);

            StatisticsAndReport.ParseResultForReport(usersInTheCity);
        }

        // This event handler updates the progress. 
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelProgress.Text = string.Format("{0}... {1}%", _bwStatus, e.ProgressPercentage.ToString());
            progressBar1.Value = e.ProgressPercentage % 100;
        }

        // This event handler deals with the results of the background operation. 
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            resultForm.richTextBoxResult.AppendText(StatisticsAndReport.GetReport());
            resultForm.ShowDialog();
            resultForm.richTextBoxResult.Clear();
        }
    }
}
