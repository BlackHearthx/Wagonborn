# Wagonborn

**Your wagon is a truck now.**

By **BlackHearthx**

Pull a moving cart and the road starts teaching you. Friends who shove from behind learn the same craft. Haul far, take the cart through the portal with you — and keep an eye on it on the map.

Wagonborn is a hauler’s skill pack for vanilla carts — no fancy new wagon models, just weight that answers to practice, crew help, a live map pin, and a cart that stays with the caravan.

**Compat:** If you also use **TeleportEverything**, that mod handles cart portals. If you also use **BetterCarts**, that mod handles buddy mass (helpers still earn Hauling here).

## What you get

| | |
| --- | --- |
| **Hauling** | Skill from pulling (or helping push) a moving cart — higher level, lighter pull |
| **Buddy push** | Friends within range lighten the load; they train Hauling too |
| **Portal cart** | Attached cart travels through portals and dungeon rune stones, then re-attaches |
| **Map pin** | Every cart on the map by default; Alt+Use hides or shows that one |
| **Steady hitch** | Longer leash while pulling; unhitch with Use (E) as usual |

## Your first session

1. Install with a mod manager (Thunderstore / r2modman). Jötunn comes with it.
2. Load a world, look at a cart, hitch with **Use (E)**.
3. **Walk** while hitched. Standing still does not train.
4. Carts show on the map by default — **Alt+Use** hides or shows that cart only.
5. Open skills — **Hauling** (Carreto) appears after the first XP.
6. Take a portal while hitched — cart should arrive with you.

## How to play

Heavier moving loads train faster. At Hauling 100, pull weight can drop by up to half — hills still matter.

In co-op, stand near a friend who is pulling. The cart gets lighter for them, and you earn Hauling while it moves. Up to four helpers count.

## Controls at a glance

| Action | Default |
| --- | --- |
| Hitch / unhitch | Look at cart → **Use (E)** |
| Mark / clear map pin | Look at cart → **Alt+Use** (each cart separate) |
| Train Hauling | Pull or help push while the cart is moving |
| Portal with cart | Walk through portal / dungeon stone while hitched |

## Compat and notes

- Host and every client need the mod.
- Do not stack **BetterCarts** buddy-mass options with Wagonborn’s Crew section (Wagonborn soft-defers if BetterCarts is loaded).
- **TeleportEverything** owns cart portal transport when both are present.
- Vanilla cart look stays vanilla.

## Como usar (PT-BR)

1. Instale pelo gerenciador de mods (Jötunn vem junto).
2. Olhe a carroça e use **E** para acoplar / soltar.
3. Ande puxando — parado não sobe **Carreto**.
4. Carroças aparecem no mapa por padrão — **Alt+E** tira ou devolve aquela só.
5. Amigos perto aliviam o peso e também sobem Carreto.
6. Portal com a carroça acoplada: ela viaja e reatacha na chegada.

## Requirements

- [BepInExPack Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)
- [Jötunn](https://thunderstore.io/c/valheim/p/ValheimModding/Jotunn/)

## Config

After first launch: `BepInEx/config/com.blackhearthx.wagonborn.cfg`

| Section | Setting | Default | Notes |
| --- | --- | --- | --- |
| Hauling | MaxSkillMassReduction | 0.50 | Mass cut at skill 100 |
| Hauling | XpPerSecond | 0.35 | Base XP while the cart moves |
| Portal | EnableCartPortal | true | Cart through portals |
| Portal | PreventCartAutoDetach | true | Longer leash while pulling |
| Crew | EnableBuddyHelp | true | Friends lighten + earn XP |
| Map | EnableCartMapPin | true | All carts on the map; Alt+Use per cart |

## Identity

| | |
| --- | --- |
| Package | `blackhearthx-Wagonborn` |
| GUID | `com.blackhearthx.wagonborn` |
| Skill | `com.blackhearthx.wagonborn.hauling` |

Do not change the GUID or skill id after release.
