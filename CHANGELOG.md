# Changelog

## 1.3.2
- Hover cleanup: compact lines, no raw $KEY_* tokens
- Map toggle is **Shift+E** (Valheim AltPlace), not Alt+E
- Buddy tip only when someone is helping; softer pin messages
- Parked carts no longer count you as a helper (no fake "1 nearby", no free weight cut)
- Shift+E no longer takes the cart from a friend who is pulling it; hidden pins are per player now
- Carts stay on the map when you walk away (last known spot; the host sees every cart)
- Hauling XP now scales with the real load, so it no longer drops as your skill rises; helpers earn it reliably in co-op
- Weight updates right away on hitch and unhitch instead of up to 5 seconds later
- A cart can no longer stay frozen after a portal if the re-hitch misses
- Steady hitch leash works even with portal transport turned off
- Map and buddy lines translated into all 20 languages
- Config: WeightXpScale replaced by LoadXpScale (240); MaxSkillMassReduction capped at 0.8

## 1.3.1
- Every cart is on the map by default; Alt+Use hides or shows that cart only (each pin is independent)

## 1.3.0
- Live map pin for carts (same idea as Hearthwife): moves in real time on the minimap and world map
- Alt+Use on a cart to mark or clear; hitching can auto-mark (config)
- Wheel icon on the pin so it reads as a cart, not a generic mark

## 1.2.2
- Removed the V hotkey hitch — attach and detach with Use (E) only

## 1.2.1
- Unhitch reliably: hotkey always drops the cart you are pulling (even when it is behind you)
- Out-of-place hitch no longer glues the cart within attach range
- Longer pull leash uses a finite distance (default 8 m) instead of never auto-dropping

## 1.2.0
- Hotkey attach / detach nearby carts (default **V**), even when a bit off the hitch
- Buddy help: friends near a pulled cart lighten the load
- Helpers also earn Hauling XP while the cart is moving
- Soft-defers hotkey + buddy mass to BetterCarts if that mod is loaded

## 1.1.0
- Attached cart teleports with you through portals and dungeon rune stones
- Cart re-attaches on arrival; exit point nudged forward so the cart fits
- Optional: cart no longer auto-detaches when you drift a few meters while pulling
- Defers to TeleportEverything if that mod is also installed (no double transport)

## 1.0.3
- Cache the attached cart for XP (no full-scene scan every frame)
- Guard XP until the Hauling skill is registered
- Safer cart hover text if localization is not ready
- Deploy path targets the Default Thunderstore profile

## 1.0.2
- Package icon with Wagonborn / Hauling Skill / blackhearthx identity

## 1.0.1
- Cart/wheel skill icon
- Extra language translations

## 1.0.0
- Hauling skill (Carreto): pull a moving cart to level up
- Higher skill makes carts easier to pull
- No visual cart upgrades
