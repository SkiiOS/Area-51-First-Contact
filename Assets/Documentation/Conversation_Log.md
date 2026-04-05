# Conversation Log - GDGoC Project

**Project:** GDGoC (Unity 6 Game)
**Team:** RAM 6GB
**Date:** April 5, 2026

---

## Session 1: Project Setup & CLAUDE.md

### Created CLAUDE.md
- Unity version: 6000.4.1f1 (Unity 6 LTS)
- Render Pipeline: Universal Render Pipeline (URP)
- Input System: New Input System configured
- Key packages: 2D Animation, Tilemap, Input System, URP, Unity MCP

### Updated .gitignore
Added Claude Code related files:
```
.claude/
CLAUDE.md
.cursor/
.cursorrules
```

---

## Session 2: Tileset Bleeding Issue

### Problem
Tileset 16x16 (DesertTilemap16x16.png) showing artifacts/garbled lines on edges when sliced.

### Root Cause
- Filter Mode: Bilinear (should be Point for pixel art)
- Wrap Mode: Repeat (should be Clamp)
- Mesh Type: Tight (should be Full Rect)

### Solution Applied
Updated `DesertTilemap16x16.png.meta`:
- `filterMode: 0` (Point)
- `wrapU: 0`, `wrapV: 0` (Clamp)
- `spriteMeshType: 0` (Full Rect)

### Next Steps Required
1. In Unity: Right-click tileset → Reimport
2. Or delete .meta file and let Unity regenerate
3. Check if artifacts are gone

### Additional Tips
- If lines persist, add 1px padding between tiles
- Use Pixel Snap on materials
- Ensure compression is disabled for pixel art

---

## Project Structure (Current)

```
Assets/
├── Animation/          # Animation assets
├── Assets/
│   ├── Character/      # Character sprites
│   ├── Ground/
│   ├── Pallete/        # Color palettes
│   ├── Tiilemap Reenderer/
│   └── TileMap/
│       └── DesertTileset16x16/
├── Documentation/      # This folder
├── Scenes/
│   └── Main Scene.unity
├── Scripts/            # (empty, ready for code)
└── Settings/           # URP settings
```

---

## Notes for Future Development

### Input Actions Available
- Move (Vector2)
- Look (Vector2)
- Attack (Button)
- Interact (Button)
- Crouch (Button)
- Jump (Button)
- Sprint (Button)

### Sprite Settings Reference
For pixel art assets, always use:
- Filter Mode: **Point (no filter)**
- Mesh Type: **Full Rect**
- Wrap Mode: **Clamp**
- Compression: **None** (for critical pixel art)

---

*Last updated: April 5, 2026*
