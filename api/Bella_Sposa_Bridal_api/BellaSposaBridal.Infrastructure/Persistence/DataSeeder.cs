using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BellaSposaBridal.Infrastructure.Persistence;

public static class DataSeeder
{
    private const string ImgBase = "https://pub-72033d04e6fe458286baded587730303.r2.dev/images";

    private static string GenerateSlug(string name)
    {
        var s = name.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
        s = Regex.Replace(s, @"\s+", "-");
        s = s.Trim('-');
        return s;
    }

    private static string HashPassword(string password)
    {
        var salt = Encoding.UTF8.GetBytes("BellaSposaBridal2024");
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public static async Task SeedAsync(AppDbContext ctx)
    {
        // Fix legacy relative and localhost URLs to point at R2
        var legacyPrefixes = new[] { "http://localhost:4200/images", "/images" };
        foreach (var old in legacyPrefixes)
        {
            if (await ctx.DressPhotos.AnyAsync(p => p.Url.StartsWith(old)))
                await ctx.DressPhotos
                    .Where(p => p.Url.StartsWith(old))
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.Url, p => ImgBase + p.Url.Substring(old.Length)));

            if (await ctx.Collections.AnyAsync(c => c.CoverImageUrl != null && c.CoverImageUrl.StartsWith(old)))
                await ctx.Collections
                    .Where(c => c.CoverImageUrl != null && c.CoverImageUrl.StartsWith(old))
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        c => c.CoverImageUrl, c => ImgBase + c.CoverImageUrl!.Substring(old.Length)));
        }

        // Backfill slugs for collections and dresses that have empty slugs (from seeder runs before slug was added)
        var collectionsWithoutSlug = await ctx.Collections.Where(c => c.Slug == null || c.Slug == "").ToListAsync();
        foreach (var c in collectionsWithoutSlug)
            c.Slug = GenerateSlug(c.Name);
        if (collectionsWithoutSlug.Any()) await ctx.SaveChangesAsync();

        var dressesWithoutSlug = await ctx.Dresses.Where(d => d.Slug == null || d.Slug == "").ToListAsync();
        foreach (var d in dressesWithoutSlug)
            d.Slug = GenerateSlug(d.Name);
        if (dressesWithoutSlug.Any()) await ctx.SaveChangesAsync();

        if (!await ctx.Dresses.AnyAsync())
            await SeedV1Async(ctx);

        if (!await ctx.Collections.AnyAsync(c => c.Id == Guid.Parse("11111111-0000-0000-0000-000000000003")))
            await SeedV2Async(ctx);

        // Seed admin user if not present
        if (!await ctx.AdminUsers.AnyAsync(u => u.Username == "admin"))
        {
            ctx.AdminUsers.Add(new AdminUser
            {
                Username = "admin",
                PasswordHash = HashPassword("adminBella")
            });
            await ctx.SaveChangesAsync();
        }

        if (!await ctx.AppointmentTypeConfigs.AnyAsync())
        {
            ctx.AppointmentTypeConfigs.AddRange(
                new AppointmentTypeConfig { Id = 0, Name = "Initial Appointment", Price = null, MainDescription = "Your first visit to our boutique. We get to know you, your vision, and your wedding story — then guide you through a curated selection of gowns.", Detail = "Up to 90 minutes · Complimentary", DisplayOrder = 0, IsActive = true },
                new AppointmentTypeConfig { Id = 1, Name = "Second Appointment", Price = null, MainDescription = "Return to refine your favourites. We narrow the styles, work on silhouette, and move closer to the dress that is truly yours.", Detail = "Up to 60 minutes · By Invitation", DisplayOrder = 1, IsActive = true },
                new AppointmentTypeConfig { Id = 2, Name = "VIP Appointment", Price = 30m, MainDescription = "An exclusive private experience — the boutique is reserved entirely for you. Champagne, personal styling, and unhurried time to find your perfect gown.", Detail = "Private Boutique · Champagne Included", DisplayOrder = 2, IsActive = true }
            );
            await ctx.SaveChangesAsync();
        }
        else
        {
            // Backfill Description and Detail for existing records that predate those fields
            var defaultData = new Dictionary<int, (string MainDescription, string DetailLine)>
            {
                [0] = (
                    MainDescription: "Your first visit to our boutique. We get to know you, your vision, and your wedding story — then guide you through a curated selection of gowns.",
                    DetailLine:      "Up to 90 minutes · Complimentary"
                ),
                [1] = (
                    MainDescription: "Return to refine your favourites. We narrow the styles, work on silhouette, and move closer to the dress that is truly yours.",
                    DetailLine:      "Up to 60 minutes · By Invitation"
                ),
                [2] = (
                    MainDescription: "An exclusive private experience — the boutique is reserved entirely for you. Champagne, personal styling, and unhurried time to find your perfect gown.",
                    DetailLine:      "Private Boutique · Champagne Included"
                ),
            };
            var needsBackfill = await ctx.AppointmentTypeConfigs
                .Where(t => t.Detail == null || t.MainDescription == null)
                .ToListAsync();
            foreach (var t in needsBackfill)
            {
                if (!defaultData.TryGetValue(t.Id, out var def)) continue;
                t.MainDescription ??= def.MainDescription;
                t.Detail          ??= def.DetailLine;
            }
            if (needsBackfill.Any()) await ctx.SaveChangesAsync();
        }

        if (!await ctx.TimeSlotConfigs.AnyAsync())
        {
            var slots = new[] { "10:00", "12:00", "14:00", "16:00", "18:00" };
            for (int i = 0; i < slots.Length; i++)
                ctx.TimeSlotConfigs.Add(new TimeSlotConfig { Id = Guid.NewGuid(), Time = slots[i], IsActive = true, DisplayOrder = i });
            await ctx.SaveChangesAsync();
        }
    }

    // â”€â”€ V1: initial 10 dresses + 2 collections â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private static async Task SeedV1Async(AppDbContext ctx)
    {
        var now = DateTime.UtcNow;

        var col2025 = new Collection
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Name = "Eternal Elegance",
            Slug = GenerateSlug("Eternal Elegance"),
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
            Slug = GenerateSlug("Celestial Dreams"),
            Season = "Autumn / Winter",
            Year = 2026,
            Description = "Dramatic silhouettes and celestial embroidery for the bride who dares to shine.",
            CoverImageUrl = $"{ImgBase}/cover-2.jpg",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        ctx.Collections.AddRange(col2025, col2026);

        if (!await ctx.AtlierInfos.AnyAsync())
        {
            ctx.AtlierInfos.Add(new AtlierInfo
            {
                Id = Guid.NewGuid(),
                Address = "29 Queenstown Road, London SW8 3RE",
                FittingDurationMinutes = 90,
                IsFittingFree = true,
                MaxGuests = 1,
                AppointmentRequired = true,
                Phone = "07466728196",
                WhatsApp = "07466728196",
                Telegram = null,
                Instagram = "@bella_sposa_bridal",
                WorkingHours = "Mon – Sun: 10:00 – 19:00",
                VipPrice = 30m,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        var d1  = Guid.Parse("22222222-0000-0000-0000-000000000001");
        var d2  = Guid.Parse("22222222-0000-0000-0000-000000000002");
        var d3  = Guid.Parse("22222222-0000-0000-0000-000000000003");
        var d4  = Guid.Parse("22222222-0000-0000-0000-000000000004");
        var d5  = Guid.Parse("22222222-0000-0000-0000-000000000005");
        var d6  = Guid.Parse("22222222-0000-0000-0000-000000000006");
        var d7  = Guid.Parse("22222222-0000-0000-0000-000000000007");
        var d8  = Guid.Parse("22222222-0000-0000-0000-000000000008");
        var d9  = Guid.Parse("22222222-0000-0000-0000-000000000009");
        var d10 = Guid.Parse("22222222-0000-0000-0000-000000000010");

        ctx.Dresses.AddRange(
            new Dress
            {
                Id = d1, Name = "Aurora", Slug = GenerateSlug("Aurora"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Minimal elegance with a royal silhouette",
                Description = "Aurora was crafted for the bride who seeks to unite restrained elegance with a truly dramatic silhouette. The structured mermaid form traces every curve with quiet confidence, while the detachable satin train allows the gown to transform from ceremony to reception in a single effortless moment. Ivory satin meets delicate lace at the corseted bodice — a marriage of strength and softness.",
                SilhouetteId = 0, Material = "Satin + Lace", CorsetType = "Structured boning",
                TrainDescription = "1.5 m detachable", Color = "Ivory", HasSleeves = false,
                Decoration = "Scattered pearl embellishment", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d2, Name = "Seraphina", Slug = GenerateSlug("Seraphina"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "A vision of celestial volume and grace",
                Description = "Seraphina is a love letter to the grand ball gown tradition. Layers upon layers of French tulle cascade from a sculptured lace bodice, creating a silhouette that fills every room with wonder. Designed for the bride who has always dreamed of arriving like a vision from another era — poised, radiant, and utterly unforgettable.",
                SilhouetteId = 1, Material = "French Tulle + Lace", CorsetType = "Boned corset",
                TrainDescription = "2 m cathedral", Color = "Soft White", HasSleeves = false,
                Decoration = "3D floral lace appliqué on bodice", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d3, Name = "Luna", Slug = GenerateSlug("Luna"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Effortless romance for the free-spirited bride",
                Description = "Luna flows rather than commands — a gown for the bride who wants to feel like herself, only more. Soft chiffon drapes naturally over an A-line silhouette, catching light with every step. The champagne tone warms the complexion and recalls the golden hour, making it a beloved choice for outdoor and destination ceremonies alike.",
                SilhouetteId = 2, Material = "Silk Chiffon", CorsetType = "Soft boning",
                TrainDescription = "0.5 m sweep", Color = "Champagne", HasSleeves = true,
                SleeveDescription = "Sheer illusion long sleeves", Decoration = "Delicate floral embroidery on sleeves",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d4, Name = "Celeste", Slug = GenerateSlug("Celeste"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Modern minimalism at its most refined",
                Description = "Celeste strips away the ornate and leaves only what is essential: perfect proportion, extraordinary fabric, and a silhouette that flatters without constraining. This sheath gown in double-faced crepe is the choice of the bride who understands that true luxury whispers. Its clean lines are a canvas for confidence.",
                SilhouetteId = 3, Material = "Double-faced Crepe", CorsetType = "Minimal internal structure",
                TrainDescription = null, Color = "Ivory", HasSleeves = false,
                Decoration = "Silk-covered buttons down the back", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d5, Name = "Iris", Slug = GenerateSlug("Iris"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Drama blooms from every step",
                Description = "Iris captures the moment when sophistication meets sensuality. The trumpet silhouette hugs the body through the bodice and hips before exploding into a lavish flared hem — a gown that rewards movement. Blush satin carries a warm, romantic luminosity, making Iris the signature choice for candlelit ceremonies.",
                SilhouetteId = 5, Material = "Stretch Satin", CorsetType = "Structured corset",
                TrainDescription = "0.8 m flared", Color = "Blush", HasSleeves = false,
                Decoration = "Beaded waist sash", CustomTailoringAvailable = false
            },
            new Dress
            {
                Id = d6, Name = "Violette", Slug = GenerateSlug("Violette"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Vintage soul in a contemporary form",
                Description = "Violette draws from the soft romanticism of the 1930s and reinterprets it for the modern bride. An empire waist gathered beneath the bust in intricate Chantilly lace creates an elongated, ethereal silhouette. Every detail — from the scalloped hem to the deep V-back — is a quiet act of poetry.",
                SilhouetteId = 4, Material = "Chantilly Lace + Silk Lining", CorsetType = "Soft cups, no boning",
                TrainDescription = "0.3 m brush", Color = "Pure White", HasSleeves = true,
                SleeveDescription = "Three-quarter lace sleeves", Decoration = "Scalloped lace hem and neckline",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d7, Name = "Elara", Slug = GenerateSlug("Elara"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Power and poise in a single silhouette",
                Description = "Elara is built for the bride who enters a room and owns it. Structured Mikado fabric holds its form with architectural precision, while the deep V-neckline and open back introduce an edge of modern daring. This is a gown that photographs beautifully from every angle — especially walking away.",
                SilhouetteId = 0, Material = "Duchess Mikado", CorsetType = "Fully boned",
                TrainDescription = "1.2 m semi-cathedral", Color = "Warm Ivory", HasSleeves = false,
                Decoration = "Crystal-beaded neckline border", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d8, Name = "Noelle", Slug = GenerateSlug("Noelle"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Floating on clouds of winter white",
                Description = "Noelle was born from a vision of a winter wedding — hushed, sacred, luminous. Layers of organza over a lace foundation create a ballgown that seems to hover above the ground. The fitted lace bodice anchors the design with intricate detail, while the full skirt evokes the pure spectacle of falling snow.",
                SilhouetteId = 1, Material = "Organza + Lace", CorsetType = "Structured with boning",
                TrainDescription = "1.8 m royal", Color = "Snow White", HasSleeves = true,
                SleeveDescription = "Detachable lace cap sleeves", Decoration = "Hand-stitched lace medallions throughout skirt",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d9, Name = "Sophia", Slug = GenerateSlug("Sophia"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "The gown time will not forget",
                Description = "Sophia is a perennial — a gown that belongs to no single season or trend. Duchess satin in a warm cream tone is cut with classical precision into an A-line that flatters every figure. It is the kind of dress that will look as beautiful in photographs fifty years from now as it does today.",
                SilhouetteId = 2, Material = "Duchess Satin", CorsetType = "Boned bodice",
                TrainDescription = "0.8 m chapel", Color = "Cream", HasSleeves = false,
                Decoration = "Satin-covered buttons, silk sash at waist", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d10, Name = "Lyra", Slug = GenerateSlug("Lyra"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Bare. Bold. Breathtaking.",
                Description = "Lyra is unapologetically modern. A sleek column silhouette in stretch lace over nude mesh creates the illusion of a second skin — a gown that celebrates rather than conceals. Designed for the bride who finds strength in simplicity and knows that the most daring choice is sometimes the most refined.",
                SilhouetteId = 3, Material = "Stretch Lace over Nude Mesh", CorsetType = "Light internal structure",
                TrainDescription = null, Color = "Nude / Ivory", HasSleeves = false,
                Decoration = "Eyelash lace trim at hem and neckline", CustomTailoringAvailable = false
            }
        );

        ctx.DressPhotos.AddRange(
            Photo(d1,  "cover-1.jpg", PhotoType.Hero, 0), Photo(d1,  "dress-1.jpg", PhotoType.Front, 1), Photo(d1,  "dress-2.jpg", PhotoType.Back, 2),
            Photo(d2,  "cover-2.jpg", PhotoType.Hero, 0), Photo(d2,  "dress-3.jpg", PhotoType.Front, 1), Photo(d2,  "dress-4.jpg", PhotoType.Back, 2),
            Photo(d3,  "cover-3.jpg", PhotoType.Hero, 0), Photo(d3,  "dress-5.jpg", PhotoType.Front, 1), Photo(d3,  "dress-6.jpg", PhotoType.Back, 2),
            Photo(d4,  "dress-1.jpg", PhotoType.Hero, 0), Photo(d4,  "dress-2.jpg", PhotoType.Front, 1), Photo(d4,  "dress-3.jpg", PhotoType.Back, 2),
            Photo(d5,  "dress-2.jpg", PhotoType.Hero, 0), Photo(d5,  "dress-4.jpg", PhotoType.Front, 1), Photo(d5,  "dress-5.jpg", PhotoType.Back, 2),
            Photo(d6,  "dress-3.jpg", PhotoType.Hero, 0), Photo(d6,  "dress-6.jpg", PhotoType.Front, 1), Photo(d6,  "dress-1.jpg", PhotoType.Back, 2),
            Photo(d7,  "dress-4.jpg", PhotoType.Hero, 0), Photo(d7,  "dress-1.jpg", PhotoType.Front, 1), Photo(d7,  "dress-2.jpg", PhotoType.Back, 2),
            Photo(d8,  "dress-5.jpg", PhotoType.Hero, 0), Photo(d8,  "dress-3.jpg", PhotoType.Front, 1), Photo(d8,  "dress-4.jpg", PhotoType.Back, 2),
            Photo(d9,  "dress-6.jpg", PhotoType.Hero, 0), Photo(d9,  "dress-2.jpg", PhotoType.Front, 1), Photo(d9,  "dress-5.jpg", PhotoType.Back, 2),
            Photo(d10, "cover-1.jpg", PhotoType.Hero, 0), Photo(d10, "dress-6.jpg", PhotoType.Front, 1), Photo(d10, "dress-3.jpg", PhotoType.Back, 2)
        );

        var commonSizes = new[] { "UK6", "UK8", "UK10", "UK12", "UK14", "UK16", "UK18", "UK20" };
        foreach (var id in new[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10 })
            ctx.DressSizes.AddRange(commonSizes.Select(s => Size(id, s)));

        // Eternal Elegance: Aurora, Luna, Celeste, Iris, Sophia
        ctx.DressCollections.AddRange(
            Link(d1, col2025.Id), Link(d3, col2025.Id), Link(d4, col2025.Id),
            Link(d5, col2025.Id), Link(d9, col2025.Id));
        // Celestial Dreams: Seraphina, Violette, Elara, Noelle, Lyra
        ctx.DressCollections.AddRange(
            Link(d2, col2026.Id), Link(d6, col2026.Id), Link(d7, col2026.Id),
            Link(d8, col2026.Id), Link(d10, col2026.Id));
        // Aurora appears in both
        ctx.DressCollections.Add(Link(d1, col2026.Id));

        foreach (var (a, b) in new (Guid, Guid)[]
        {
            (d1, d7),   // Aurora ↔ Elara (both Mermaid)
            (d2, d8),   // Seraphina ↔ Noelle (both BallGown)
            (d3, d9),   // Luna ↔ Sophia (both A-Line)
            (d4, d10),  // Celeste ↔ Lyra (both Sheath)
            (d1, d3),   // Aurora ↔ Luna
            (d5, d6),   // Iris ↔ Violette
        })
        {
            ctx.RelatedDresses.Add(new RelatedDress { DressId = a, RelatedDressId = b });
            ctx.RelatedDresses.Add(new RelatedDress { DressId = b, RelatedDressId = a });
        }

        await ctx.SaveChangesAsync();
    }

    // â”€â”€ V2: 10 new dresses + 2 new collections â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private static async Task SeedV2Async(AppDbContext ctx)
    {
        var now = DateTime.UtcNow;

        // â”€â”€ Collections â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var col3 = new Collection
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000003"),
            Name = "Golden Hour",
            Slug = GenerateSlug("Golden Hour"),
            Season = "Autumn / Winter",
            Year = 2025,
            Description = "Warm, amber-lit silhouettes for the bride who finds beauty in the fleeting — a collection steeped in romance and the quiet magic of the last light of day.",
            CoverImageUrl = $"{ImgBase}/cover-3.jpg",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var col4 = new Collection
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000004"),
            Name = "Pure Avant-Garde",
            Slug = GenerateSlug("Pure Avant-Garde"),
            Season = "Spring / Summer",
            Year = 2026,
            Description = "Architecture meets couture. Clean geometry, unexpected proportions, and uncompromising fabrics for the bride who sees her wedding as a statement of self.",
            CoverImageUrl = $"{ImgBase}/dress-4.jpg",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        ctx.Collections.AddRange(col3, col4);

        // â”€â”€ Dress IDs â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var d11 = Guid.Parse("22222222-0000-0000-0000-000000000011"); // Daphne  — Tea Length
        var d12 = Guid.Parse("22222222-0000-0000-0000-000000000012"); // Cleo    — Mini
        var d13 = Guid.Parse("22222222-0000-0000-0000-000000000013"); // Vivienne — Mermaid
        var d14 = Guid.Parse("22222222-0000-0000-0000-000000000014"); // Bianca  — A-Line
        var d15 = Guid.Parse("22222222-0000-0000-0000-000000000015"); // Clara   — Empire
        var d16 = Guid.Parse("22222222-0000-0000-0000-000000000016"); // Margot  — Tea Length
        var d17 = Guid.Parse("22222222-0000-0000-0000-000000000017"); // Estelle — Ball Gown
        var d18 = Guid.Parse("22222222-0000-0000-0000-000000000018"); // Petra   — Sheath
        var d19 = Guid.Parse("22222222-0000-0000-0000-000000000019"); // Mathilde — Trumpet
        var d20 = Guid.Parse("22222222-0000-0000-0000-000000000020"); // Roxanne — Mini

        // Existing dress IDs for cross-linking
        var d1  = Guid.Parse("22222222-0000-0000-0000-000000000001");
        var d2  = Guid.Parse("22222222-0000-0000-0000-000000000002");
        var d3  = Guid.Parse("22222222-0000-0000-0000-000000000003");
        var d4  = Guid.Parse("22222222-0000-0000-0000-000000000004");
        var d5  = Guid.Parse("22222222-0000-0000-0000-000000000005");
        var d6  = Guid.Parse("22222222-0000-0000-0000-000000000006");
        var d7  = Guid.Parse("22222222-0000-0000-0000-000000000007");
        var d8  = Guid.Parse("22222222-0000-0000-0000-000000000008");
        var d9  = Guid.Parse("22222222-0000-0000-0000-000000000009");

        // â”€â”€ Dresses â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        ctx.Dresses.AddRange(
            new Dress
            {
                Id = d11, Name = "Daphne", Slug = GenerateSlug("Daphne"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Where vintage charm meets modern grace",
                Description = "Daphne reimagines the tea-length silhouette for the contemporary bride — an unexpected choice that is quietly extraordinary. Dupioni silk with a structured velvet bodice creates a rich textural dialogue, while the voluminous mid-calf skirt moves with effortless levity. Daphne is for the woman who has always known she is something apart.",
                SilhouetteId = 6, Material = "Dupioni Silk + Velvet", CorsetType = "Boned bustier",
                TrainDescription = null, Color = "Dusty Rose", HasSleeves = false,
                Decoration = "Hand-sewn velvet ribbon sash, lace trim at hem", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d12, Name = "Cleo", Slug = GenerateSlug("Cleo"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Bold brevity for the fearless bride",
                Description = "Cleo rewrites every rule. A micro-structured mini in crystal-embellished Mikado challenges the very notion of what a wedding gown can be — and wins. Whether worn for a city hall ceremony or a rooftop reception, Cleo demands attention and delivers nothing short of awe. The bride who chooses Cleo knows exactly who she is.",
                SilhouetteId = 7, Material = "Embellished Mikado", CorsetType = "Structured cups with boning",
                TrainDescription = null, Color = "Ivory", HasSleeves = false,
                Decoration = "All-over crystal micro-embroidery", CustomTailoringAvailable = false
            },
            new Dress
            {
                Id = d13, Name = "Vivienne", Slug = GenerateSlug("Vivienne"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "A fluid sculpture in the finest silk",
                Description = "Vivienne moves the way only the finest silk can — as though pulled by some invisible, elegant tide. The bias-cut mermaid silhouette traces the body without clinging, with a hand-pleated asymmetric shoulder detail that catches light like a whisper. This is a gown built for the bride who knows that effortlessness is the ultimate luxury.",
                SilhouetteId = 0, Material = "Bias-cut Silk Charmeuse", CorsetType = "Internal softly structured support",
                TrainDescription = "0.9 m sweep", Color = "Deep Ivory", HasSleeves = false,
                Decoration = "Hand-pleated asymmetric shoulder drape", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d14, Name = "Bianca", Slug = GenerateSlug("Bianca"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Romance in every layer",
                Description = "Bianca was made for open skies — a garden ceremony, a clifftop in Tuscany, a meadow at dusk. Silk georgette in the softest blush cascades from a lace-appliqué bodice in an A-line that flatters and flows in equal measure. Flutter sleeves in sheer georgette add a lightness that seems to defy gravity. Bianca is romance distilled.",
                SilhouetteId = 2, Material = "Silk Georgette + Lace", CorsetType = "Lightly boned",
                TrainDescription = "0.6 m chapel", Color = "Blush", HasSleeves = true,
                SleeveDescription = "Flutter cap sleeves in sheer georgette", Decoration = "Floral lace appliqué at bodice",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d15, Name = "Clara", Slug = GenerateSlug("Clara"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "A goddess silhouette for the modern bride",
                Description = "Clara channels antiquity and reinterprets it for the woman of today. The empire waist, gathered high beneath the bust in delicately beaded silk organza, creates a line of infinite grace. Sheer bishop sleeves add a romantic softness, and the silhouette falls naturally to the floor — unhurried, unforced, and undeniably beautiful.",
                SilhouetteId = 4, Material = "Silk Organza + Satin Lining", CorsetType = "Soft internal cups",
                TrainDescription = "0.4 m sweep", Color = "Ivory", HasSleeves = true,
                SleeveDescription = "Sheer organza bishop sleeves", Decoration = "Beadwork at empire seam",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d16, Name = "Margot", Slug = GenerateSlug("Margot"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "French elegance distilled to its essence",
                Description = "Margot is everything you imagine when you picture a bride in the South of France — entirely unaffected, entirely unforgettable. French guipure lace is cut into a tea-length silhouette with three-quarter sleeves and a scalloped hem that grazes the mid-calf. The V-back is a quiet masterstroke. Margot does not announce herself. She simply arrives.",
                SilhouetteId = 6, Material = "French Guipure Lace + Silk Lining", CorsetType = "Built-in boning",
                TrainDescription = null, Color = "Soft White", HasSleeves = true,
                SleeveDescription = "Three-quarter guipure lace sleeves", Decoration = "Scalloped lace hem and deep V-back",
                CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d17, Name = "Estelle", Slug = GenerateSlug("Estelle"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "The fairytale your childhood dreamed of",
                Description = "Estelle is the ballgown that makes every room fall silent. Duchess satin forms a perfectly proportioned corseted bodice from which layers of organza erupt in a full cathedral skirt. Hand-folded organza rosettes bloom at the hip — each one individually placed, each one a testament to the craft behind the gown. Estelle is not simply worn. She is inhabited.",
                SilhouetteId = 1, Material = "Duchess Satin + Layered Organza", CorsetType = "Fully boned with steel channels",
                TrainDescription = "2.5 m cathedral", Color = "Pure White", HasSleeves = false,
                Decoration = "Hand-folded organza rosettes at the hip", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d18, Name = "Petra", Slug = GenerateSlug("Petra"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Precision cut for the architectural bride",
                Description = "Petra is a masterclass in the art of restraint. Structured silk crepe is cut with absolute precision into a sheath that defines the figure without diminishing it. The deep cowl back — a single unbroken fall of fabric — transforms simplicity into drama. Petra is the gown for the bride who believes that confidence is the only ornament a woman truly needs.",
                SilhouetteId = 3, Material = "Structured Silk Crepe", CorsetType = "Invisible internal boning",
                TrainDescription = null, Color = "Champagne", HasSleeves = false,
                Decoration = "Deep cowl back with covered satin buttons", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d19, Name = "Mathilde", Slug = GenerateSlug("Mathilde"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "French lace for the bride who dares to bloom",
                Description = "Mathilde pays tribute to the great tradition of French couture lace. Guipure in full floral relief covers every inch of the trumpet silhouette, which follows the body closely before flaring into a dramatic hem. It is a gown that rewards slowness — the closer one looks, the more extraordinary the detail becomes. Mathilde is a lifetime's work of craft worn for one perfect day.",
                SilhouetteId = 5, Material = "Guipure Lace + Silk Lining", CorsetType = "Structured corset with boning",
                TrainDescription = "0.6 m flared", Color = "Antique White", HasSleeves = false,
                Decoration = "All-over guipure floral lace with scalloped hem", CustomTailoringAvailable = true
            },
            new Dress
            {
                Id = d20, Name = "Roxanne", Slug = GenerateSlug("Roxanne"), IsActive = true, CreatedAt = now, UpdatedAt = now,
                Tagline = "Unapologetically radiant",
                Description = "Roxanne is an editorial in fabric form. A micro-mini in crystal-scattered organza sits above a structured satin bodice that catches every available light source and turns it into spectacle. Roxanne does not ask for permission to be extraordinary. She is for the bride who has already decided she is the most beautiful person in every room — because she is.",
                SilhouetteId = 7, Material = "Silk Organza + Crystal Mesh", CorsetType = "Structured cups",
                TrainDescription = null, Color = "White", HasSleeves = false,
                Decoration = "Scattered crystal embellishment throughout skirt", CustomTailoringAvailable = false
            }
        );

        // â”€â”€ Photos (rotating through the 6 dress images) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        ctx.DressPhotos.AddRange(
            Photo(d11, "dress-1.jpg", PhotoType.Hero, 0), Photo(d11, "dress-3.jpg", PhotoType.Front, 1), Photo(d11, "dress-5.jpg", PhotoType.Back, 2),
            Photo(d12, "dress-2.jpg", PhotoType.Hero, 0), Photo(d12, "dress-4.jpg", PhotoType.Front, 1), Photo(d12, "dress-6.jpg", PhotoType.Back, 2),
            Photo(d13, "dress-3.jpg", PhotoType.Hero, 0), Photo(d13, "dress-5.jpg", PhotoType.Front, 1), Photo(d13, "dress-1.jpg", PhotoType.Back, 2),
            Photo(d14, "dress-4.jpg", PhotoType.Hero, 0), Photo(d14, "dress-6.jpg", PhotoType.Front, 1), Photo(d14, "dress-2.jpg", PhotoType.Back, 2),
            Photo(d15, "dress-5.jpg", PhotoType.Hero, 0), Photo(d15, "dress-1.jpg", PhotoType.Front, 1), Photo(d15, "dress-3.jpg", PhotoType.Back, 2),
            Photo(d16, "dress-6.jpg", PhotoType.Hero, 0), Photo(d16, "dress-2.jpg", PhotoType.Front, 1), Photo(d16, "dress-4.jpg", PhotoType.Back, 2),
            Photo(d17, "cover-3.jpg", PhotoType.Hero, 0), Photo(d17, "dress-1.jpg", PhotoType.Front, 1), Photo(d17, "dress-5.jpg", PhotoType.Back, 2),
            Photo(d18, "dress-4.jpg", PhotoType.Hero, 0), Photo(d18, "dress-2.jpg", PhotoType.Front, 1), Photo(d18, "dress-6.jpg", PhotoType.Back, 2),
            Photo(d19, "dress-1.jpg", PhotoType.Hero, 0), Photo(d19, "dress-4.jpg", PhotoType.Front, 1), Photo(d19, "dress-6.jpg", PhotoType.Back, 2),
            Photo(d20, "dress-2.jpg", PhotoType.Hero, 0), Photo(d20, "dress-3.jpg", PhotoType.Front, 1), Photo(d20, "dress-5.jpg", PhotoType.Back, 2)
        );

        // â”€â”€ Sizes â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var fullSizes  = new[] { "UK6", "UK8", "UK10", "UK12", "UK14", "UK16", "UK18", "UK20" };
        var shortSizes = new[] { "UK6", "UK8", "UK10", "UK12", "UK14" };
        foreach (var id in new[] { d11, d13, d14, d15, d16, d17, d18, d19 })
            ctx.DressSizes.AddRange(fullSizes.Select(s => Size(id, s)));
        foreach (var id in new[] { d12, d20 }) // Mini styles — smaller range
            ctx.DressSizes.AddRange(shortSizes.Select(s => Size(id, s)));

        // â”€â”€ Collection links â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        // Golden Hour: Daphne, Vivienne, Clara, Estelle, Roxanne
        ctx.DressCollections.AddRange(
            Link(d11, col3.Id), Link(d13, col3.Id), Link(d15, col3.Id),
            Link(d17, col3.Id), Link(d20, col3.Id));
        // Pure Avant-Garde: Cleo, Bianca, Margot, Petra, Mathilde
        ctx.DressCollections.AddRange(
            Link(d12, col4.Id), Link(d14, col4.Id), Link(d16, col4.Id),
            Link(d18, col4.Id), Link(d19, col4.Id));
        // Cross-collection bridges
        ctx.DressCollections.Add(Link(d11, col4.Id)); // Daphne bridges both (tea-length in both moods)
        ctx.DressCollections.Add(Link(d13, col4.Id)); // Vivienne bridges both (minimal mermaid)

        // â”€â”€ Related dresses â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var related = new (Guid A, Guid B)[]
        {
            // Within new dresses — by silhouette
            (d11, d16),  // Daphne ↔ Margot  (both Tea Length)
            (d12, d20),  // Cleo ↔ Roxanne   (both Mini)
            (d13, d19),  // Vivienne ↔ Mathilde (body-skimming, dramatic)
            (d14, d15),  // Bianca ↔ Clara   (flowing, romantic)
            (d17, d18),  // Estelle ↔ Petra  (contrast: full vs minimal)

            // New ↔ Existing — by silhouette
            (d13, d1),   // Vivienne ↔ Aurora  (both Mermaid)
            (d13, d7),   // Vivienne ↔ Elara   (both Mermaid)
            (d17, d2),   // Estelle ↔ Seraphina (both BallGown)
            (d17, d8),   // Estelle ↔ Noelle   (both BallGown)
            (d14, d3),   // Bianca ↔ Luna      (both A-Line)
            (d14, d9),   // Bianca ↔ Sophia    (both A-Line)
            (d18, d4),   // Petra ↔ Celeste    (both Sheath)
            (d15, d6),   // Clara ↔ Violette   (both Empire)
            (d19, d5),   // Mathilde ↔ Iris    (both Trumpet)
            (d11, d6),   // Daphne ↔ Violette  (vintage romantic mood)
            (d16, d6),   // Margot ↔ Violette  (lace + vintage mood)
        };

        foreach (var (a, b) in related)
        {
            ctx.RelatedDresses.Add(new RelatedDress { DressId = a, RelatedDressId = b });
            ctx.RelatedDresses.Add(new RelatedDress { DressId = b, RelatedDressId = a });
        }

        await ctx.SaveChangesAsync();
    }

    // â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private static DressPhoto Photo(Guid dressId, string file, PhotoType type, int order) => new()
    {
        Id       = Guid.NewGuid(),
        DressId  = dressId,
        Url      = $"{ImgBase}/{file}",
        AltText  = null,
        Type     = type,
        Order    = order
    };

    private static DressSize Size(Guid dressId, string size) => new()
    {
        Id      = Guid.NewGuid(),
        DressId = dressId,
        Size    = size
    };

    private static DressCollection Link(Guid dressId, Guid colId) => new()
    {
        DressId      = dressId,
        CollectionId = colId
    };
}


