# Kamil Default Model — AI Generation Prompt

## Generation Target
**Model**: Kamil the Camel (Default Skin)  
**Tools**: Leonardo.AI or Meshy AI  
**Output Format**: Low-poly 3D model (OBJ/FBX ready for manual cleanup)

---

## Primary Prompt (Leonardo.AI/Midjourney Style)

```
Stylized, low-poly 3D model of a charming, slightly goofy camel character.
Cartoonish proportions: exaggerated squat legs, long neck, oversized head.
Sandy tan/brown body (RGB 210,180,140) with brown accents.
Accessories:
  - Aviator goggles on forehead (chrome/gold frames, clear lenses)
  - Colorful Moroccan saddle blanket across back with geometric patterns, hanging tassels
  - Red leather bridle around snout/face
Expression: happy with open grin, big expressive glossy eyes, slight underbite.
Style: Low-poly, chunky geometry, strong readable silhouette matching Crossy Road / Subway Surfers.
Polygon count: target under 1200 triangles, clean topology (mostly quads).
Views: front, side, back, 3/4 perspective.
Background: clean white or transparent.
Game asset, Unity-ready, high detail concept art.
```

---

## Detailed Reference Points

### Facial Features
- **Eyes**: Large, round, glossy, bright (add specular highlights for "life")
- **Grin**: Wide open-mouthed smile, showing teeth or tongue tip
- **Expression**: Happy but slightly goofy (eyebrows raised slightly)
- **Underbite**: Slight lower jaw protrusion, cute

### Body Proportions
- **Head**: ~30% of body height (oversized)
- **Neck**: Long, curved, elegant
- **Body**: Compact, rounded barrel
- **Legs**: Short, thick, squat
- **Tail**: Thin, slightly curved

### Accessories Detail
- **Goggles**: Aviator-style (round), positioned on forehead (can tilt/rotate in rig)
- **Saddle Blanket**: Ornate geometric patterns, bright colors (reds, golds, blues), tassels hanging front/back
- **Bridle**: Red leather, visible over snout, rope details

### Color Reference
- **Body**: Sandy tan (RGB 210, 180, 140) to warm brown (RGB 160, 120, 80)
- **Eyes**: Glossy black pupils with white highlights, inner light reflection
- **Saddle**: Geometric colors (magenta, turquoise, gold) on cream/tan base
- **Bridle**: Rich red (RGB 200, 50, 50)
- **Goggles**: Metallic chrome/gold frames

---

## Post-Generation Cleanup Checklist

After AI generation, manually:

1. **Retopology**: If mesh is inefficient, retopo to clean quads
2. **Topology Check**: Ensure deformation-friendly loops (especially around joints, neck, legs)
3. **Simplification**: Remove unnecessary details, maintain silhouette
4. **Tri Count**: Target <1,200 tris (verify in Blender)
5. **UV Layout**: Create single 1024x1024 atlas, fit all materials
6. **Normal Map**: Bake from high-poly to maintain detail with low geometry
7. **Export**: FBX 2020, clean node hierarchy

---

## Success Criteria
- [ ] Camel silhouette is immediately recognizable
- [ ] Accessories clearly visible (goggles, saddle, bridle)
- [ ] Happy, goofy expression matches description
- [ ] Polygon count verified <1,200 tris
- [ ] Topology is animation-ready (clean edge loops, proper deformation)
- [ ] Colors match reference palette
- [ ] Clean UV layout, no overlaps
