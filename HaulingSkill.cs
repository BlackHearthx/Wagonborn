using Jotunn.Configs;
using Jotunn.Managers;
using UnityEngine;

namespace Wagonborn
{
    internal static class HaulingSkill
    {
        internal const string Identifier = "com.blackhearthx.wagonborn.hauling";

        internal static Skills.SkillType SkillType { get; private set; }

        internal static void Register()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
            PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
        }

        private static void OnVanillaPrefabsAvailable()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;

            SkillType = SkillManager.Instance.AddSkill(new SkillConfig
            {
                Identifier = Identifier,
                Name = "$skill_wagonborn_hauling",
                Description = "$skill_wagonborn_hauling_desc",
                IncreaseStep = 1f,
                Icon = FindCartIcon() ?? CreateWheelIcon()
            });
        }

        internal static float GetLevel(Player player)
        {
            if (player == null || SkillType == Skills.SkillType.None)
            {
                return 0f;
            }

            return player.GetSkillFactor(SkillType) * 100f;
        }

        private static Sprite FindCartIcon()
        {
            string[] names = { "Cart", "piece_cart", "CartWood" };
            for (int i = 0; i < names.Length; i++)
            {
                GameObject prefab = PrefabManager.Instance.GetPrefab(names[i]);
                if (prefab == null)
                {
                    continue;
                }

                Piece piece = prefab.GetComponent<Piece>();
                if (piece != null && piece.m_icon != null)
                {
                    return piece.m_icon;
                }
            }

            return null;
        }

        private static Sprite CreateWheelIcon()
        {
            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            Color rim = new Color(0.45f, 0.28f, 0.12f, 1f);
            Color wood = new Color(0.72f, 0.50f, 0.26f, 1f);
            Color iron = new Color(0.55f, 0.55f, 0.58f, 1f);
            Color clear = Color.clear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float r = Mathf.Sqrt((dx * dx) + (dy * dy));
                    Color pixel = clear;

                    if (r < 30f && r > 24f)
                    {
                        pixel = rim;
                    }
                    else if (r <= 24f && r > 8f)
                    {
                        float ang = Mathf.Atan2(dy, dx);
                        float spoke = Mathf.Abs(Mathf.Sin(ang * 4f));
                        pixel = spoke > 0.72f ? wood : clear;
                    }

                    if (r <= 8f)
                    {
                        pixel = iron;
                    }

                    if (r <= 3.2f)
                    {
                        pixel = new Color(0.25f, 0.25f, 0.27f, 1f);
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}
