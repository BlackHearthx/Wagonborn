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

            Add(loc, "English", "Hauling",
                "Earned by pulling a moving cart. Higher Hauling makes carts easier to pull.",
                "Hauling $1 — pull weight $2%");

            Add(loc, "Portuguese_Brazilian", "Carreto",
                "Sobe puxando uma carroça em movimento. Quanto maior o nível, mais leve ela fica.",
                "Carreto $1 — peso de puxar $2%");

            Add(loc, "Portuguese_European", "Carreto",
                "Aumenta ao puxar um carro em movimento. Quanto mais alto o nível, mais leve fica.",
                "Carreto $1 — peso a puxar $2%");

            Add(loc, "Spanish", "Acarrero",
                "Sube al tirar de un carro en movimiento. A más nivel, más fácil de arrastrar.",
                "Acarrero $1 — peso de tiro $2%");

            Add(loc, "French", "Traction",
                "Augmente en tirant une charrette en mouvement. Plus le niveau est élevé, plus elle est légère.",
                "Traction $1 — poids de traction $2%");

            Add(loc, "German", "Ziehen",
                "Steigt, wenn du einen fahrenden Karren ziehst. Höheres Level macht ihn leichter.",
                "Ziehen $1 — Zuggewicht $2%");

            Add(loc, "Italian", "Traino",
                "Sale tirando un carro in movimento. Più alto è il livello, più è leggero.",
                "Traino $1 — peso di traino $2%");

            Add(loc, "Dutch", "Slepen",
                "Stijgt door een rijdende kar te trekken. Hoe hoger, hoe lichter de kar.",
                "Slepen $1 — trekkgewicht $2%");

            Add(loc, "Swedish", "Dragning",
                "Ökar när du drar en vagn i rörelse. Högre nivå gör den lättare.",
                "Dragning $1 — dragvikt $2%");

            Add(loc, "Polish", "Wożenie",
                "Rośnie podczas ciągnięcia jadącego wozu. Wyższy poziom ułatwia ciągnięcie.",
                "Wożenie $1 — ciężar ciągu $2%");

            Add(loc, "Russian", "Возка",
                "Растёт, когда вы везёте телегу. Чем выше навык, тем легче тянуть.",
                "Возка $1 — вес тяги $2%");

            Add(loc, "Ukrainian", "Перевезення",
                "Зростає, коли ви тягнете віз у русі. Вищий рівень робить його легшим.",
                "Перевезення $1 — вага тяги $2%");

            Add(loc, "Czech", "Tahání",
                "Stoupá tažením jedoucího vozu. Vyšší úroveň ho odlehčí.",
                "Tahání $1 — tahová váha $2%");

            Add(loc, "Hungarian", "Húzás",
                "Mozgó szekér húzásával nő. Magasabb szint könnyebbé teszi.",
                "Húzás $1 — húzósúly $2%");

            Add(loc, "Romanian", "Transport",
                "Crește trăgând un car în mișcare. Nivelul mai mare îl face mai ușor.",
                "Transport $1 — greutate de tracțiune $2%");

            Add(loc, "Turkish", "Çekme",
                "Hareket halindeki bir arabayı çekerek artar. Seviye yükseldikçe araba hafifler.",
                "Çekme $1 — çekiş ağırlığı $2%");

            Add(loc, "Chinese", "拉车",
                "拉动行驶中的推车可提升。等级越高，推车越轻。",
                "拉车 $1 — 牵引重量 $2%");

            Add(loc, "Chinese_Trad", "拉車",
                "拉動行進中的推車可提升。等級越高，推車越輕。",
                "拉車 $1 — 牽引重量 $2%");

            Add(loc, "Japanese", "運搬",
                "動いている荷車を引くと上がる。レベルが高いほど引きやすくなる。",
                "運搬 $1 — 引き重さ $2%");

            Add(loc, "Korean", "운반",
                "움직이는 수레를 끌면 오른다. 레벨이 높을수록 수레가 가벼워진다.",
                "운반 $1 — 견인 무게 $2%");
        }

        private static void Add(CustomLocalization loc, string language, string skill, string description, string hover)
        {
            loc.AddTranslation(language, new Dictionary<string, string>
            {
                { "skill_wagonborn_hauling", skill },
                { "skill_wagonborn_hauling_desc", description },
                { "wagonborn_hover", hover }
            });
        }
    }
}
