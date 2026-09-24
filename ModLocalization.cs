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
            Add(loc, "English",
                "Hauling",
                "Earned by pulling or helping push a moving cart. Higher Hauling makes carts easier to pull.",
                "Hauling $1 — pull weight $2%",
                "Cart travels with you",
                "Portals take the cart",
                "Friends nearby lighten the load — and train Hauling");

            Add(loc, "Portuguese_Brazilian",
                "Carreto",
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
