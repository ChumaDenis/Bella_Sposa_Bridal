using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Persistence;

public static class DataSeeder
{
    private const string ImgBase = "http://localhost:4200/images";

    public static async Task SeedAsync(AppDbContext ctx)
    {
        if (await ctx.Dresses.AnyAsync()) return;

        var now = DateTime.UtcNow;

        // ── Collections ────────────────────────────────────────────────
        var col2025 = new Collection
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Name = "Eternal Elegance",
            Season = "Spring / Summer",
            Year = 2025,
            Description = "A timeless collection inspired by classical architecture and the soft light of Mediterranean mornings.",
            CoverImageUrl = $"{ImgBase}/cover-1.jpg",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var col2026 = new Collection
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
            Name = "Celestial Dreams",
            Season = "Autumn / Winter",
            Year = 2026,
            Description = "Dramatic silhouettes and celestial embroidery for the bride who dares to shine.",
            CoverImageUrl = $"{ImgBase}/cover-2.jpg",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        ctx.Collections.AddRange(col2025, col2026);

        // ── Atelier info ───────────────────────────────────────────────
        if (!await ctx.AtlierInfos.AnyAsync())
        {
            ctx.AtlierInfos.Add(new AtlierInfo
            {
                Id = Guid.NewGuid(),
                Address = "14 Bridal Lane, Kyiv, Ukraine",
                FittingDurationMinutes = 90,
                IsFittingFree = true,
                MaxGuests = 3,
                AppointmentRequired = true,
                Phone = "+38 (044) 000-00-00",
                WhatsApp = "+38 (099) 000-00-00",
                Telegram = "@bella_sposa_bridal",
                Instagram = "@bella_sposa_bridal",
                WorkingHours = "Mon – Sat: 10:00 – 19:00",
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // ── Helper ────────────────────────────────────────────────────
        static DressPhoto Photo(Guid dressId, string file, PhotoType type, int order) => new()
        {
            Id = Guid.NewGuid(),
            DressId = dressId,
            Url = $"{ImgBase}/{file}",
            AltText = null,
            Type = type,
            Order = order
        };

        static DressSize Size(Guid dressId, string size) => new()
        {
            Id = Guid.NewGuid(),
            DressId = dressId,
            Size = size
        };

        static DressCollection Link(Guid dressId, Guid colId) => new()
        {
            DressId = dressId,
            CollectionId = colId
        };

        // ── Dresses ───────────────────────────────────────────────────
        var d1 = Guid.Parse("22222222-0000-0000-0000-000000000001");
        var d2 = Guid.Parse("22222222-0000-0000-0000-000000000002");
        var d3 = Guid.Parse("22222222-0000-0000-0000-000000000003");
        var d4 = Guid.Parse("22222222-0000-0000-0000-000000000004");
        var d5 = Guid.Parse("22222222-0000-0000-0000-000000000005");
        var d6 = Guid.Parse("22222222-0000-0000-0000-000000000006");
        var d7 = Guid.Parse("22222222-0000-0000-0000-000000000007");
        var d8 = Guid.Parse("22222222-0000-0000-0000-000000000008");
        var d9 = Guid.Parse("22222222-0000-0000-0000-000000000009");
        var d10 = Guid.Parse("22222222-0000-0000-0000-000000000010");

        var dresses = new List<Dress>
        {
            new()
            {
                Id = d1, Name = "Aurora", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Minimal elegance with a royal silhouette",
                Description = "Aurora was crafted for the bride who seeks to unite restrained elegance with a truly dramatic silhouette. The structured mermaid form traces every curve with quiet confidence, while the detachable satin train allows the gown to transform from ceremony to reception in a single effortless moment. Ivory satin meets delicate lace at the corseted bodice — a marriage of strength and softness.",
                Silhouette = Silhouette.Mermaid, Material = "Satin + Lace", CorsetType = "Structured boning",
                TrainDescription = "1.5 m detachable", Color = "Ivory", HasSleeves = false,
                Decoration = "Scattered pearl embellishment", CustomTailoringAvailable = true
            },
            new()
            {
                Id = d2, Name = "Seraphina", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "A vision of celestial volume and grace",
                Description = "Seraphina is a love letter to the grand ball gown tradition. Layers upon layers of French tulle cascade from a sculptured lace bodice, creating a silhouette that fills every room with wonder. Designed for the bride who has always dreamed of arriving like a vision from another era — poised, radiant, and utterly unforgettable.",
                Silhouette = Silhouette.BallGown, Material = "French Tulle + Lace", CorsetType = "Boned corset",
                TrainDescription = "2 m cathedral", Color = "Soft White", HasSleeves = false,
                Decoration = "3D floral lace appliqué on bodice", CustomTailoringAvailable = true
            },
            new()
            {
                Id = d3, Name = "Luna", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Effortless romance for the free-spirited bride",
                Description = "Luna flows rather than commands — a gown for the bride who wants to feel like herself, only more. Soft chiffon drapes naturally over an A-line silhouette, catching light with every step. The champagne tone warms the complexion and recalls the golden hour, making it a beloved choice for outdoor and destination ceremonies alike.",
                Silhouette = Silhouette.ALine, Material = "Silk Chiffon", CorsetType = "Soft boning",
                TrainDescription = "0.5 m sweep", Color = "Champagne", HasSleeves = true,
                SleeveDescription = "Sheer illusion long sleeves", Decoration = "Delicate floral embroidery on sleeves",
                CustomTailoringAvailable = true
            },
            new()
            {
                Id = d4, Name = "Celeste", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Modern minimalism at its most refined",
                Description = "Celeste strips away the ornate and leaves only what is essential: perfect proportion, extraordinary fabric, and a silhouette that flatters without constraining. This sheath gown in double-faced crepe is the choice of the bride who understands that true luxury whispers. Its clean lines are a canvas for confidence.",
                Silhouette = Silhouette.Sheath, Material = "Double-faced Crepe", CorsetType = "Minimal internal structure",
                TrainDescription = null, Color = "Ivory", HasSleeves = false,
                Decoration = "Silk-covered buttons down the back", CustomTailoringAvailable = true
            },
            new()
            {
                Id = d5, Name = "Iris", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Drama blooms from every step",
                Description = "Iris captures the moment when sophistication meets sensuality. The trumpet silhouette hugs the body through the bodice and hips before exploding into a lavish flared hem — a gown that rewards movement. Blush satin carries a warm, romantic luminosity, making Iris the signature choice for candlelit ceremonies.",
                Silhouette = Silhouette.Trumpet, Material = "Stretch Satin", CorsetType = "Structured corset",
                TrainDescription = "0.8 m flared", Color = "Blush", HasSleeves = false,
                Decoration = "Beaded waist sash", CustomTailoringAvailable = false
            },
            new()
            {
                Id = d6, Name = "Violette", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Vintage soul in a contemporary form",
                Description = "Violette draws from the soft romanticism of the 1930s and reinterprets it for the modern bride. An empire waist gathered beneath the bust in intricate Chantilly lace creates an elongated, ethereal silhouette. Every detail — from the scalloped hem to the deep V-back — is a quiet act of poetry.",
                Silhouette = Silhouette.Empire, Material = "Chantilly Lace + Silk Lining", CorsetType = "Soft cups, no boning",
                TrainDescription = "0.3 m brush", Color = "Pure White", HasSleeves = true,
                SleeveDescription = "Three-quarter lace sleeves", Decoration = "Scalloped lace hem and neckline",
                CustomTailoringAvailable = true
            },
            new()
            {
                Id = d7, Name = "Elara", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Power and poise in a single silhouette",
                Description = "Elara is built for the bride who enters a room and owns it. Structured Mikado fabric holds its form with architectural precision, while the deep V-neckline and open back introduce an edge of modern daring. This is a gown that photographs beautifully from every angle — especially walking away.",
                Silhouette = Silhouette.Mermaid, Material = "Duchess Mikado", CorsetType = "Fully boned",
                TrainDescription = "1.2 m semi-cathedral", Color = "Warm Ivory", HasSleeves = false,
                Decoration = "Crystal-beaded neckline border", CustomTailoringAvailable = true
            },
            new()
            {
                Id = d8, Name = "Noelle", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Floating on clouds of winter white",
                Description = "Noelle was born from a vision of a winter wedding — hushed, sacred, luminous. Layers of organza over a lace foundation create a ballgown that seems to hover above the ground. The fitted lace bodice anchors the design with intricate detail, while the full skirt evokes the pure spectacle of falling snow.",
                Silhouette = Silhouette.BallGown, Material = "Organza + Lace", CorsetType = "Structured with boning",
                TrainDescription = "1.8 m royal", Color = "Snow White", HasSleeves = true,
                SleeveDescription = "Detachable lace cap sleeves", Decoration = "Hand-stitched lace medallions throughout skirt",
                CustomTailoringAvailable = true
            },
            new()
            {
                Id = d9, Name = "Sophia", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "The gown time will not forget",
                Description = "Sophia is a perennial — a gown that belongs to no single season or trend. Duchess satin in a warm cream tone is cut with classical precision into an A-line that flatters every figure. It is the kind of dress that will look as beautiful in photographs fifty years from now as it does today.",
                Silhouette = Silhouette.ALine, Material = "Duchess Satin", CorsetType = "Boned bodice",
                TrainDescription = "0.8 m chapel", Color = "Cream", HasSleeves = false,
                Decoration = "Satin-covered buttons, silk sash at waist", CustomTailoringAvailable = true
            },
            new()
            {
                Id = d10, Name = "Lyra", IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Bare. Bold. Breathtaking.",
                Description = "Lyra is unapologetically modern. A sleek column silhouette in stretch lace over nude mesh creates the illusion of a second skin — a gown that celebrates rather than conceals. Designed for the bride who finds strength in simplicity and knows that the most daring choice is sometimes the most refined.",
                Silhouette = Silhouette.Sheath, Material = "Stretch Lace over Nude Mesh", CorsetType = "Light internal structure",
                TrainDescription = null, Color = "Nude / Ivory", HasSleeves = false,
                Decoration = "Eyelash lace trim at hem and neckline", CustomTailoringAvailable = false
            }
        };

        ctx.Dresses.AddRange(dresses);

        // ── Photos ────────────────────────────────────────────────────
        ctx.DressPhotos.AddRange(
            // Aurora
            Photo(d1, "cover-1.jpg", PhotoType.Hero, 0),
            Photo(d1, "dress-1.jpg", PhotoType.Front, 1),
            Photo(d1, "dress-2.jpg", PhotoType.Back, 2),
            // Seraphina
            Photo(d2, "cover-2.jpg", PhotoType.Hero, 0),
            Photo(d2, "dress-3.jpg", PhotoType.Front, 1),
            Photo(d2, "dress-4.jpg", PhotoType.Back, 2),
            // Luna
            Photo(d3, "cover-3.jpg", PhotoType.Hero, 0),
            Photo(d3, "dress-5.jpg", PhotoType.Front, 1),
            Photo(d3, "dress-6.jpg", PhotoType.Back, 2),
            // Celeste
            Photo(d4, "dress-1.jpg", PhotoType.Hero, 0),
            Photo(d4, "dress-2.jpg", PhotoType.Front, 1),
            Photo(d4, "dress-3.jpg", PhotoType.Back, 2),
            // Iris
            Photo(d5, "dress-2.jpg", PhotoType.Hero, 0),
            Photo(d5, "dress-4.jpg", PhotoType.Front, 1),
            Photo(d5, "dress-5.jpg", PhotoType.Back, 2),
            // Violette
            Photo(d6, "dress-3.jpg", PhotoType.Hero, 0),
            Photo(d6, "dress-6.jpg", PhotoType.Front, 1),
            Photo(d6, "dress-1.jpg", PhotoType.Back, 2),
            // Elara
            Photo(d7, "dress-4.jpg", PhotoType.Hero, 0),
            Photo(d7, "dress-1.jpg", PhotoType.Front, 1),
            Photo(d7, "dress-2.jpg", PhotoType.Back, 2),
            // Noelle
            Photo(d8, "dress-5.jpg", PhotoType.Hero, 0),
            Photo(d8, "dress-3.jpg", PhotoType.Front, 1),
            Photo(d8, "dress-4.jpg", PhotoType.Back, 2),
            // Sophia
            Photo(d9, "dress-6.jpg", PhotoType.Hero, 0),
            Photo(d9, "dress-2.jpg", PhotoType.Front, 1),
            Photo(d9, "dress-5.jpg", PhotoType.Back, 2),
            // Lyra
            Photo(d10, "cover-1.jpg", PhotoType.Hero, 0),
            Photo(d10, "dress-6.jpg", PhotoType.Front, 1),
            Photo(d10, "dress-3.jpg", PhotoType.Back, 2)
        );

        // ── Sizes ─────────────────────────────────────────────────────
        var commonSizes = new[] { "XS", "S", "M", "L", "XL" };
        foreach (var id in new[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10 })
            ctx.DressSizes.AddRange(commonSizes.Select(s => Size(id, s)));

        // ── Collection links ──────────────────────────────────────────
        // Eternal Elegance 2025: Aurora, Luna, Celeste, Iris, Sophia
        ctx.DressCollections.AddRange(
            Link(d1, col2025.Id), Link(d3, col2025.Id),
            Link(d4, col2025.Id), Link(d5, col2025.Id), Link(d9, col2025.Id));

        // Celestial Dreams 2026: Seraphina, Violette, Elara, Noelle, Lyra
        ctx.DressCollections.AddRange(
            Link(d2, col2026.Id), Link(d6, col2026.Id),
            Link(d7, col2026.Id), Link(d8, col2026.Id), Link(d10, col2026.Id));

        // Aurora appears in both collections
        ctx.DressCollections.Add(Link(d1, col2026.Id));

        // ── Related dresses (bidirectional) ───────────────────────────
        var related = new (Guid A, Guid B)[]
        {
            (d1, d7), // Aurora ↔ Elara  (both mermaid)
            (d2, d8), // Seraphina ↔ Noelle  (both ballgown)
            (d3, d9), // Luna ↔ Sophia  (both A-line)
            (d4, d10), // Celeste ↔ Lyra  (both sheath)
            (d1, d3), // Aurora ↔ Luna
            (d5, d6), // Iris ↔ Violette
        };

        foreach (var (a, b) in related)
        {
            ctx.RelatedDresses.Add(new RelatedDress { DressId = a, RelatedDressId = b });
            ctx.RelatedDresses.Add(new RelatedDress { DressId = b, RelatedDressId = a });
        }

        await ctx.SaveChangesAsync();
    }
}
