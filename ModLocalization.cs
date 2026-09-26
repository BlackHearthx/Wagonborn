using System.Collections.Generic;
using Jotunn.Entities;
using Jotunn.Managers;

namespace Wagonborn
{
    internal static class ModLocalization
    {
        internal static void Register()
        {
            CustomLocalization loc = LocalizationManager.Instance.GetLocalization();

            // skill, desc, hover, cartPortal, portalHint, buddyHint
            Add(loc, "English", "Hauling",
                "Earned by pulling or helping push a moving cart. Higher Hauling makes carts easier to pull.",
                "Hauling $1 — pull weight $2%",
                "Cart travels with you",
                "Portals take the cart",
                "Friends nearby lighten the load — and train Hauling");

            Add(loc, "Portuguese_Brazilian", "Carreto",
                "Sobe puxando ou ajudando a empurrar uma carroça em movimento. Quanto maior o nível, mais leve ela fica.",
                "Carreto $1 — peso de puxar $2%",
                "Carroça vem junto",
                "Portais levam a carroça",
                "Amigos por perto aliviam o peso — e sobem Carreto");

            Add(loc, "Portuguese_European",
                "Carreto",
                "Aumenta ao puxar ou ajudar a empurrar um carro em movimento. Quanto mais alto o nível, mais leve fica.",
                "Carreto $1 — peso a puxar $2%",
                "Carro vem consigo",
                "Portais levam o carro",
                "Amigos por perto aliviam o peso — e sobem Carreto");

            Add(loc, "Spanish",
                "Acarrero",
                "Sube al tirar o ayudar a empujar un carro en movimiento. A más nivel, más fácil de arrastrar.",
                "Acarrero $1 — peso de tiro $2%",
                "El carro viaja contigo",
                "Los portales llevan el carro",
                "Amigos cerca alivian la carga — y suben Acarrero");

            Add(loc, "French",
                "Traction",
                "Augmente en tirant ou en aidant à pousser une charrette en mouvement. Plus le niveau est élevé, plus elle est légère.",
                "Traction $1 — poids de traction $2%",
                "La charrette voyage avec vous",
                "Les portails emportent la charrette",
                "Les amis proches allègent la charge — et montent Traction");

            Add(loc, "German",
                "Ziehen",
                "Steigt, wenn du einen fahrenden Karren ziehst oder mitschiebst. Höheres Level macht ihn leichter.",
                "Ziehen $1 — Zuggewicht $2%",
                "Karren reist mit",
                "Portale nehmen den Karren mit",
                "Freunde in der Nähe erleichtern die Last — und trainieren Ziehen");

            Add(loc, "Italian",
                "Traino",
                "Sale tirando o aiutando a spingere un carro in movimento. Più alto è il livello, più è leggero.",
                "Traino $1 — peso di traino $2%",
                "Il carro viaggia con te",
                "I portali portano il carro",
                "Amici vicini alleggeriscono il carico — e allenano Traino");

            Add(loc, "Dutch",
                "Slepen",
                "Stijgt door een rijdende kar te trekken of mee te duwen. Hoe hoger, hoe lichter de kar.",
                "Slepen $1 — trekkgewicht $2%",
                "Kar reist mee",
                "Portalen nemen de kar mee",
                "Vrienden in de buurt verlichten de last — en trainen Slepen");

            Add(loc, "Swedish",
                "Dragning",
                "Ökar när du drar eller hjälper till att knuffa en vagn i rörelse. Högre nivå gör den lättare.",
                "Dragning $1 — dragvikt $2%",
                "Vagnen följer med",
                "Portaler tar vagnen",
                "Vänner nära lättar lasten — och tränar Dragning");

            Add(loc, "Polish",
                "Wożenie",
                "Rośnie podczas ciągnięcia lub pomocy w pchaniu jadącego wozu. Wyższy poziom ułatwia ciągnięcie.",
                "Wożenie $1 — ciężar ciągu $2%",
                "Wóz podróżuje z tobą",
                "Portale zabierają wóz",
                "Znajomi w pobliżu odciążają — i trenują Wożenie");

            Add(loc, "Russian",
                "Возка",
                "Растёт, когда вы везёте телегу или помогаете толкать. Чем выше навык, тем легче тянуть.",
                "Возка $1 — вес тяги $2%",
                "Телега идёт с вами",
                "Порталы забирают телегу",
                "Друзья рядом облегчают груз — и качают Возку");

            Add(loc, "Ukrainian",
                "Перевезення",
                "Зростає, коли ви тягнете віз або допомагаєте штовхати. Вищий рівень робить його легшим.",
                "Перевезення $1 — вага тяги $2%",
                "Віз іде з вами",
                "Портали забирають віз",
                "Друзі поруч полегшують вагу — і качають Перевезення");

            Add(loc, "Czech",
                "Tahání",
                "Stoupá tažením nebo pomocí tlačení jedoucího vozu. Vyšší úroveň ho odlehčí.",
                "Tahání $1 — tahová váha $2%",
                "Vůz cestuje s tebou",
                "Portály berou vůz",
                "Přátelé poblíž odlehčí náklad — a trénují Tahání");

            Add(loc, "Hungarian",
                "Húzás",
                "Mozgó szekér húzásával vagy tolásával nő. Magasabb szint könnyebbé teszi.",
                "Húzás $1 — húzósúly $2%",
                "A szekér veled utazik",
                "A portálok elviszik a szekeret",
                "Közeli barátok könnyítik a terhet — és fejlesztik a Húzást");

            Add(loc, "Romanian",
                "Transport",
                "Crește trăgând sau ajutând la împins un car în mișcare. Nivelul mai mare îl face mai ușor.",
                "Transport $1 — greutate de tracțiune $2%",
                "Carul călătorește cu tine",
                "Portalurile iau carul",
                "Prietenii apropiați ușurează sarcina — și antrenează Transport");

            Add(loc, "Turkish",
                "Çekme",
                "Hareket halindeki arabayı çekerek veya itmeye yardım ederek artar. Seviye yükseldikçe araba hafifler.",
                "Çekme $1 — çekiş ağırlığı $2%",
                "Araba seninle gider",
                "Portallar arabayı alır",
                "Yakındaki dostlar yükü hafifletir — ve Çekme yükseltir");

            Add(loc, "Chinese",
                "拉车",
                "拉动或帮忙推动行驶中的推车可提升。等级越高，推车越轻。",
                "拉车 $1 — 牵引重量 $2%",
                "推车与你同行",
                "传送门带走推车",
                "附近的朋友减轻负担 — 也提升拉车");

            Add(loc, "Chinese_Trad",
                "拉車",
                "拉動或幫忙推動行進中的推車可提升。等級越高，推車越輕。",
                "拉車 $1 — 牽引重量 $2%",
                "推車與你同行",
                "傳送門帶走推車",
                "附近的朋友減輕負擔 — 也提升拉車");

            Add(loc, "Japanese",
                "運搬",
                "動いている荷車を引く、または押す手伝いをすると上がる。レベルが高いほど引きやすくなる。",
                "運搬 $1 — 引き重さ $2%",
                "荷車も一緒に移動",
                "ポータルで荷車も運ぶ",
                "近くの仲間が負担を軽くする — 運搬も上がる");

            Add(loc, "Korean",
                "운반",
                "움직이는 수레를 끌거나 밀어 도우면 오른다. 레벨이 높을수록 수레가 가벼워진다.",
                "운반 $1 — 견인 무게 $2%",
                "수레가 함께 이동",
                "포털이 수레를 데려감",
                "근처 친구가 짐을 덜어 줌 — 운반도 오른다");

            AddMapPin(loc, "English",
                "On the map  [<color=yellow><b>Shift+E</b></color>] hide",
                "Off the map  [<color=yellow><b>Shift+E</b></color>] show",
                "On the map",
                "Off the map",
                "Cart",
                "$1 nearby — lighter haul");
            AddMapPin(loc, "Portuguese_Brazilian",
                "No mapa  [<color=yellow><b>Shift+E</b></color>] tirar",
                "Fora do mapa  [<color=yellow><b>Shift+E</b></color>] marcar",
                "No mapa",
                "Fora do mapa",
                "Carroça",
                "$1 por perto — carga mais leve");
            AddMapPin(loc, "Portuguese_European",
                "No mapa  [<color=yellow><b>Shift+E</b></color>] tirar",
                "Fora do mapa  [<color=yellow><b>Shift+E</b></color>] marcar",
                "No mapa",
                "Fora do mapa",
                "Carro",
                "$1 por perto — carga mais leve");
            AddMapPin(loc, "Spanish",
                "En el mapa  [<color=yellow><b>Shift+E</b></color>] quitar",
                "Fuera del mapa  [<color=yellow><b>Shift+E</b></color>] marcar",
                "En el mapa",
                "Fuera del mapa",
                "Carro",
                "$1 cerca — carga más ligera");
            AddMapPin(loc, "French",
                "Sur la carte  [<color=yellow><b>Shift+E</b></color>] retirer",
                "Hors carte  [<color=yellow><b>Shift+E</b></color>] afficher",
                "Sur la carte",
                "Hors carte",
                "Charrette",
                "$1 proches — charge plus légère");
            AddMapPin(loc, "German",
                "Auf der Karte  [<color=yellow><b>Shift+E</b></color>] ausblenden",
                "Nicht auf der Karte  [<color=yellow><b>Shift+E</b></color>] zeigen",
                "Auf der Karte",
                "Nicht auf der Karte",
                "Karren",
                "$1 in der Nähe — leichtere Last");

            const string K = "[<color=yellow><b>Shift+E</b></color>]";
            AddMapPin(loc, "Italian",
                "Sulla mappa  " + K + " nascondi", "Fuori mappa  " + K + " mostra",
                "Sulla mappa", "Fuori mappa", "Carro",
                "$1 vicini — carico più leggero");
            AddMapPin(loc, "Dutch",
                "Op de kaart  " + K + " verbergen", "Niet op de kaart  " + K + " tonen",
                "Op de kaart", "Niet op de kaart", "Kar",
                "$1 dichtbij — lichtere vracht");
            AddMapPin(loc, "Swedish",
                "På kartan  " + K + " dölj", "Inte på kartan  " + K + " visa",
                "På kartan", "Inte på kartan", "Vagn",
                "$1 nära — lättare last");
            AddMapPin(loc, "Polish",
                "Na mapie  " + K + " ukryj", "Poza mapą  " + K + " pokaż",
                "Na mapie", "Poza mapą", "Wóz",
                "$1 w pobliżu — lżejszy ładunek");
            AddMapPin(loc, "Russian",
                "На карте  " + K + " скрыть", "Не на карте  " + K + " показать",
                "На карте", "Не на карте", "Телега",
                "Рядом: $1 — груз легче");
            AddMapPin(loc, "Ukrainian",
                "На мапі  " + K + " сховати", "Не на мапі  " + K + " показати",
                "На мапі", "Не на мапі", "Віз",
                "Поруч: $1 — вантаж легший");
            AddMapPin(loc, "Czech",
                "Na mapě  " + K + " skrýt", "Mimo mapu  " + K + " zobrazit",
                "Na mapě", "Mimo mapu", "Vůz",
                "$1 poblíž — lehčí náklad");
            AddMapPin(loc, "Hungarian",
                "A térképen  " + K + " elrejt", "Nincs a térképen  " + K + " mutat",
                "A térképen", "Nincs a térképen", "Szekér",
                "$1 a közelben — könnyebb teher");
            AddMapPin(loc, "Romanian",
                "Pe hartă  " + K + " ascunde", "Nu e pe hartă  " + K + " arată",
                "Pe hartă", "Nu e pe hartă", "Car",
                "$1 aproape — încărcătură mai ușoară");
            AddMapPin(loc, "Turkish",
                "Haritada  " + K + " gizle", "Haritada değil  " + K + " göster",
                "Haritada", "Haritada değil", "Araba",
                "Yakında $1 kişi — yük hafif");
            AddMapPin(loc, "Chinese",
                "在地图上  " + K + " 隐藏", "不在地图上  " + K + " 显示",
                "在地图上", "不在地图上", "推车",
                "附近 $1 人 — 负担更轻");
            AddMapPin(loc, "Chinese_Trad",
                "在地圖上  " + K + " 隱藏", "不在地圖上  " + K + " 顯示",
                "在地圖上", "不在地圖上", "推車",
                "附近 $1 人 — 負擔更輕");
            AddMapPin(loc, "Japanese",
                "地図に表示中  " + K + " 隠す", "地図に非表示  " + K + " 表示",
                "地図に表示", "地図から外した", "荷車",
                "近くに $1 人 — 荷が軽い");
            AddMapPin(loc, "Korean",
                "지도에 표시됨  " + K + " 숨기기", "지도에 없음  " + K + " 표시",
                "지도에 표시", "지도에서 숨김", "수레",
                "근처 $1명 — 짐이 가벼워짐");
        }

        private static void AddMapPin(
            CustomLocalization loc,
            string language,
            string hoverOn,
            string hoverOff,
            string on,
            string off,
            string name,
            string buddyHover)
        {
            loc.AddTranslation(language, new Dictionary<string, string>
            {
                { "wagonborn_mappin_hover_on", hoverOn },
                { "wagonborn_mappin_hover_off", hoverOff },
                { "wagonborn_mappin_on", on },
                { "wagonborn_mappin_off", off },
                { "wagonborn_mappin_name", name },
                { "wagonborn_buddy_hover", buddyHover }
            });
        }

        private static void Add(
            CustomLocalization loc,
            string language,
            string skill,
            string description,
            string hover,
            string cartPortal,
            string cartPortalHint,
            string buddyHint)
        {
            loc.AddTranslation(language, new Dictionary<string, string>
            {
                { "skill_wagonborn_hauling", skill },
                { "skill_wagonborn_hauling_desc", description },
                { "wagonborn_hover", hover },
                { "wagonborn_cart_portal", cartPortal },
                { "wagonborn_cart_portal_hint", cartPortalHint },
                { "wagonborn_buddy_hint", buddyHint }
            });
        }
    }
}
