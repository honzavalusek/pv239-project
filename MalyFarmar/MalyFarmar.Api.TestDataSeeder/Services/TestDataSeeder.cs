using Common.Enums;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.TestDataSeeder.Services;

public class TestDataSeeder : IDataSeeder
{
    private readonly MalyFarmarDbContext _context;

    public TestDataSeeder(MalyFarmarDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("Deleting Existing Data...");
        await DeleteExisting();

        Console.WriteLine("Seeding Users...");
        await SeedUsersAsync();

        Console.WriteLine("Seeding Products...");
        await SeedProductsAsync();

        Console.WriteLine("Seeding Orders...");
        await SeedOrdersAsync();
    }

    private async Task DeleteExisting()
    {
        foreach (var tableName in new[] { "OrderItems", "Orders", "Products", "Users" })
        {
#pragma warning disable EF1002
            await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
            await _context.Database.ExecuteSqlRawAsync($"DELETE FROM sqlite_sequence WHERE name='{tableName}'");
#pragma warning restore EF1002
        }
    }

    private async Task SeedUsersAsync()
    {
        var newUsers = new List<User>
        {
            new User
            {
                FirstName = "Janka",
                LastName = "Nováková",
                Email = "janka.nov@gmail.com",
                PhoneNumber = "+420 123 456 789",
                LocationLatitude = 49.195061,
                LocationLongitude = 16.606836,
            },
            new User
            {
                FirstName = "Petr",
                LastName = "Prempl",
                Email = "peta.p@seznam.cz",
                PhoneNumber = "+420 987 654 321",
                LocationLatitude = 49.191238,
                LocationLongitude = 16.612467,
            },
            new User
            {
                FirstName = "Tomáš",
                LastName = "Procházka",
                Email = "tomas.prochazka@gmail.com",
                PhoneNumber = "+420 601 234 567",
                LocationLatitude = 49.196824,
                LocationLongitude = 16.599743,
            },
            new User
            {
                FirstName = "Lenka",
                LastName = "Dvořáková",
                Email = "lenka.dvorak@seznam.cz",
                PhoneNumber = "+420 702 345 678",
                LocationLatitude = 49.192376,
                LocationLongitude = 16.603912,
            },
            new User
            {
                FirstName = "Michal",
                LastName = "Svoboda",
                Email = "michal.svoboda@email.cz",
                PhoneNumber = "+420 728 456 789",
                LocationLatitude = 49.198247,
                LocationLongitude = 16.617584,
            },
            new User
            {
                FirstName = "Veronika",
                LastName = "Horáková",
                Email = "veronika.h@centrum.cz",
                PhoneNumber = "+420 608 567 890",
                LocationLatitude = 49.202183,
                LocationLongitude = 16.608953,
            },
            new User
            {
                FirstName = "Jan",
                LastName = "Kučera",
                Email = "jan.kucera@post.cz",
                PhoneNumber = "+420 777 678 901",
                LocationLatitude = 49.193865,
                LocationLongitude = 16.594217,
            },
            new User
            {
                FirstName = "Markéta",
                LastName = "Veselá",
                Email = "marketka.vesela@gmail.com",
                PhoneNumber = "+420 603 789 012",
                LocationLatitude = 49.593778,
                LocationLongitude = 17.250879,
            },
            new User
            {
                FirstName = "David",
                LastName = "Novotný",
                Email = "david.novotny@seznam.cz",
                PhoneNumber = "+420 724 890 123",
                LocationLatitude = 49.595623,
                LocationLongitude = 17.258341,
            },
            new User
            {
                FirstName = "Lucie",
                LastName = "Marková",
                Email = "lucie.markova@email.cz",
                PhoneNumber = "+420 775 901 234",
                LocationLatitude = 49.591497,
                LocationLongitude = 17.246152,
            },
            new User
            {
                FirstName = "Jakub",
                LastName = "Černý",
                Email = "jakub.cerny@centrum.cz",
                PhoneNumber = "+420 606 012 345",
                LocationLatitude = 49.589246,
                LocationLongitude = 17.253765,
            },
            new User
            {
                FirstName = "Karolína",
                LastName = "Šimková",
                Email = "karolina.simkova@post.cz",
                PhoneNumber = "+420 731 123 456",
                LocationLatitude = 49.596852,
                LocationLongitude = 17.242839,
            },
            new User
            {
                FirstName = "Martin",
                LastName = "Pospíšil",
                Email = "martin.pospisil@gmail.com",
                PhoneNumber = "+420 604 234 567",
                LocationLatitude = 50.073658,
                LocationLongitude = 14.418540,
            },
            new User
            {
                FirstName = "Tereza",
                LastName = "Němcová",
                Email = "tereza.nemcova@seznam.cz",
                PhoneNumber = "+420 722 345 678",
                LocationLatitude = 50.076231,
                LocationLongitude = 14.424678,
            },
            new User
            {
                FirstName = "Filip",
                LastName = "Marek",
                Email = "filip.marek@email.cz",
                PhoneNumber = "+420 776 456 789",
                LocationLatitude = 50.071042,
                LocationLongitude = 14.412396,
            },
            new User
            {
                FirstName = "Kateřina",
                LastName = "Benešová",
                Email = "katerina.b@centrum.cz",
                PhoneNumber = "+420 607 567 890",
                LocationLatitude = 50.079724,
                LocationLongitude = 14.421853,
            },
            new User
            {
                FirstName = "Ondřej",
                LastName = "Doležal",
                Email = "ondrej.dolezal@post.cz",
                PhoneNumber = "+420 730 678 901",
                LocationLatitude = 50.076985,
                LocationLongitude = 14.409231,
            }
        };

        await _context.Users.AddRangeAsync(newUsers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedProductsAsync()
    {
        var newProducts = new List<Product>
        {
            new Product
            {
                Name = "Jahody",
                Description = "Čerstvé jahody z farmy",
                TotalAmount = 5,
                RemainingAmount = 5,
                Unit = "Kg",
                PricePerUnit = 120,
                SellerId = 1
            },
            new Product
            {
                Name = "Brambory",
                Description = "Domácí brambory, odrůda Adéla",
                TotalAmount = 10,
                RemainingAmount = 10,
                Unit = "Kg",
                PricePerUnit = 22,
                SellerId = 2
            },
            new Product
            {
                Name = "Med květový",
                Description = "Přírodní včelí med z Vysočiny",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 180,
                SellerId = 3
            },
            new Product
            {
                Name = "Mrkev",
                Description = "Bio mrkev, čerstvě sklizená",
                TotalAmount = 4,
                RemainingAmount = 4,
                Unit = "Kg",
                PricePerUnit = 45,
                SellerId = 1
            },
            new Product
            {
                Name = "Jablka Jonagold",
                Description = "Šťavnatá jablka z jižní Moravy",
                TotalAmount = 8,
                RemainingAmount = 8,
                Unit = "Kg",
                PricePerUnit = 35,
                SellerId = 4
            },
            new Product
            {
                Name = "Domácí vajíčka",
                Description = "Vajíčka z volného chovu, velikost M",
                TotalAmount = 30,
                RemainingAmount = 30,
                Unit = "Ks",
                PricePerUnit = 5,
                SellerId = 2
            },
            new Product
            {
                Name = "Česnek",
                Description = "Odrůda Dukát, vhodný na uskladnění",
                TotalAmount = 1.5m,
                RemainingAmount = 0,
                Unit = "Kg",
                PricePerUnit = 160,
                SellerId = 5
            },
            new Product
            {
                Name = "Kuřecí maso",
                Description = "Čerstvé kuřecí z malého hospodářství",
                TotalAmount = 4,
                RemainingAmount = 4,
                Unit = "Kg",
                PricePerUnit = 145,
                SellerId = 6
            },
            new Product
            {
                Name = "Cuketa",
                Description = "Mladá cuketa z vlastní zahrady",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 40,
                SellerId = 3
            },
            new Product
            {
                Name = "Domácí tvaroh",
                Description = "Plnotučný farmářský tvaroh",
                TotalAmount = 2,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 95,
                SellerId = 7
            },
            new Product
            {
                Name = "Cibule",
                Description = "Sušená cibule, vhodná na dlouhodobé skladování",
                TotalAmount = 5,
                RemainingAmount = 5,
                Unit = "Kg",
                PricePerUnit = 28,
                SellerId = 8
            },
            new Product
            {
                Name = "Borůvky",
                Description = "Lesní borůvky, ručně sbírané",
                TotalAmount = 2,
                RemainingAmount = 0.3m,
                Unit = "Kg",
                PricePerUnit = 220,
                SellerId = 4
            },
            new Product
            {
                Name = "Domácí chléb",
                Description = "Kváskový chléb z celožitné mouky",
                TotalAmount = 10,
                RemainingAmount = 10,
                Unit = "Ks",
                PricePerUnit = 65,
                SellerId = 9
            },
            new Product
            {
                Name = "Bylinkový sirup",
                Description = "Sirup z máty a meduňky, domácí výroba",
                TotalAmount = 5,
                RemainingAmount = 5,
                Unit = "L",
                PricePerUnit = 110,
                SellerId = 10
            },
            new Product
            {
                Name = "Papriky",
                Description = "Sladké papriky různých barev",
                TotalAmount = 2.5m,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 75,
                SellerId = 1
            },
            new Product
            {
                Name = "Domácí klobásy",
                Description = "Tradiční uzené klobásy bez přidaných látek",
                TotalAmount = 3,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 210,
                SellerId = 11
            },
            new Product
            {
                Name = "Ořechy vlašské",
                Description = "Vlašské ořechy loupané, sklizeň 2024",
                TotalAmount = 2,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 195,
                SellerId = 5
            },
            new Product
            {
                Name = "Domácí švestková povidla",
                Description = "Tradiční povidla bez přidaného cukru",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 170,
                SellerId = 8
            },
            new Product
            {
                Name = "Čerstvý kozí sýr",
                Description = "Jemný kozí sýr s bylinkami",
                TotalAmount = 1.5m,
                RemainingAmount = 0.2m,
                Unit = "Kg",
                PricePerUnit = 280,
                SellerId = 12
            },
            new Product
            {
                Name = "Rajčata cherry",
                Description = "Sladká cherry rajčata z foliovníku",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 90,
                SellerId = 3
            },
            new Product
            {
                Name = "Farmářské máslo",
                Description = "Domácí máslo z pasterizované smetany",
                TotalAmount = 1,
                RemainingAmount = 1,
                Unit = "Kg",
                PricePerUnit = 220,
                SellerId = 7
            },
            new Product
            {
                Name = "Hlíva ústřičná",
                Description = "Čerstvě sklizená hlíva z vlastní produkce",
                TotalAmount = 1.5m,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 150,
                SellerId = 9
            },
            new Product
            {
                Name = "Domácí marmeláda",
                Description = "Jahodovo-rebarborová marmeláda",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 160,
                SellerId = 10
            },
            new Product
            {
                Name = "Dýně Hokkaido",
                Description = "Bio dýně vhodná na polévku i pečení",
                TotalAmount = 4,
                RemainingAmount = 4,
                Unit = "Kg",
                PricePerUnit = 45,
                SellerId = 6
            },
            new Product
            {
                Name = "Domácí těstoviny",
                Description = "Ručně vyráběné vaječné těstoviny",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 130,
                SellerId = 13
            },
            new Product
            {
                Name = "Zelí bílé",
                Description = "Čerstvé křupavé zelí, odrůda Megaton",
                TotalAmount = 5,
                RemainingAmount = 5,
                Unit = "Kg",
                PricePerUnit = 25,
                SellerId = 2
            },
            new Product
            {
                Name = "Sušené bylinky",
                Description = "Mix bylinek na vaření (tymián, rozmarýn, oregano)",
                TotalAmount = 0.5m,
                RemainingAmount = 0.5m,
                Unit = "Kg",
                PricePerUnit = 300,
                SellerId = 14
            },
            new Product
            {
                Name = "Jablečný mošt",
                Description = "Přírodní jablečný mošt bez konzervantů",
                TotalAmount = 4,
                RemainingAmount = 4,
                Unit = "L",
                PricePerUnit = 65,
                SellerId = 4
            },
            new Product
            {
                Name = "Kváskový žitný chléb",
                Description = "Tradiční receptura, pečeno v peci na dřevo",
                TotalAmount = 8,
                RemainingAmount = 8,
                Unit = "Ks",
                PricePerUnit = 75,
                SellerId = 9
            },
            new Product
            {
                Name = "Králičí maso",
                Description = "Z domácího chovu, krmeno bez GMO",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 190,
                SellerId = 11
            },
            new Product
            {
                Name = "Květový med s pláství",
                Description = "Surový nevčelařený med přímo z pláství",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 250,
                SellerId = 3
            },
            new Product
            {
                Name = "Domácí pálenka slivovice",
                Description = "Tradiční moravská slivovice, 52%",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "L",
                PricePerUnit = 350,
                SellerId = 15
            },
            new Product
            {
                Name = "Červená řepa",
                Description = "Čerstvá červená řepa bio kvality",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "Kg",
                PricePerUnit = 35,
                SellerId = 5
            },
            new Product
            {
                Name = "Ovčí jogurt",
                Description = "Krémový jogurt z ovčího mléka",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 180,
                SellerId = 12
            },
            new Product
            {
                Name = "Špenát čerstvý",
                Description = "Mladé špenátové listy z ekologického pěstování",
                TotalAmount = 1,
                RemainingAmount = 1,
                Unit = "Kg",
                PricePerUnit = 120,
                SellerId = 6
            },
            new Product
            {
                Name = "Domácí džem borůvkový",
                Description = "Z lesních borůvek, nízký obsah cukru",
                TotalAmount = 1.5m,
                RemainingAmount = 1,
                Unit = "Kg",
                PricePerUnit = 215,
                SellerId = 8
            },
            new Product
            {
                Name = "Dýňová semínka",
                Description = "Sušená nesolená semínka, bohatá na zinek",
                TotalAmount = 1,
                RemainingAmount = 1,
                Unit = "Kg",
                PricePerUnit = 180,
                SellerId = 16
            },
            new Product
            {
                Name = "Pórek",
                Description = "Čerstvý pórek, dlouhý bílý stonek",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 60,
                SellerId = 2
            },
            new Product
            {
                Name = "Domácí lívanečky",
                Description = "Předpřipravené lívanečky, stačí ohřát",
                TotalAmount = 20,
                RemainingAmount = 20,
                Unit = "Ks",
                PricePerUnit = 8,
                SellerId = 13
            },
            new Product
            {
                Name = "Olivový olej",
                Description = "Extra panenský, lisovaný za studena",
                TotalAmount = 3,
                RemainingAmount = 3,
                Unit = "L",
                PricePerUnit = 250,
                SellerId = 17
            },
            new Product
            {
                Name = "Fazole zelené",
                Description = "Čerstvé zelené fazolky, ručně sklizené",
                TotalAmount = 2,
                RemainingAmount = 2,
                Unit = "Kg",
                PricePerUnit = 85,
                SellerId = 4
            }
        };

        await _context.Products.AddRangeAsync(newProducts);
        await _context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync()
    {
        var newOrders = new List<Order>
    {
        new Order
        {
            PickUpAt = null,
            BuyerId = 2,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    Amount = 2,
                    PricePerUnit = 120
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(1),
            BuyerId = 3,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    Amount = 1,
                    PricePerUnit = 120
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(2),
            BuyerId = 4,
            StatusId = OrderStatusEnum.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    Amount = 1,
                    PricePerUnit = 120
                },
            }
        },
        // New orders
        new Order
        {
            PickUpAt = null,
            BuyerId = 5,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 2,
                    Amount = 4,
                    PricePerUnit = 22
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(1),
            BuyerId = 6,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 4,
                    Amount = 2,
                    PricePerUnit = 45
                },
            }
        },
        new Order
        {
            PickUpAt = null,
            BuyerId = 7,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 5,
                    Amount = 3,
                    PricePerUnit = 35
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(3),
            BuyerId = 8,
            StatusId = OrderStatusEnum.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 8,
                    Amount = 2,
                    PricePerUnit = 145
                },
            }
        },
        new Order
        {
            PickUpAt = null,
            BuyerId = 9,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 10,
                    Amount = 0.5m,
                    PricePerUnit = 95
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(2),
            BuyerId = 10,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 11,
                    Amount = 2,
                    PricePerUnit = 28
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(1),
            BuyerId = 11,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 12,
                    Amount = 1,
                    PricePerUnit = 220
                },
            }
        },
        new Order
        {
            PickUpAt = null,
            BuyerId = 12,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 13,
                    Amount = 3,
                    PricePerUnit = 65
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(4),
            BuyerId = 13,
            StatusId = OrderStatusEnum.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 14,
                    Amount = 2,
                    PricePerUnit = 110
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(1),
            BuyerId = 14,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 15,
                    Amount = 1,
                    PricePerUnit = 75
                },
            }
        },
        new Order
        {
            PickUpAt = null,
            BuyerId = 15,
            StatusId = OrderStatusEnum.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 16,
                    Amount = 1,
                    PricePerUnit = 210
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(5),
            BuyerId = 16,
            StatusId = OrderStatusEnum.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 17,
                    Amount = 0.5m,
                    PricePerUnit = 195
                },
            }
        },
        new Order
        {
            PickUpAt = DateTime.UtcNow.AddDays(2),
            BuyerId = 17,
            StatusId = OrderStatusEnum.PickUpSet,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 18,
                    Amount = 1,
                    PricePerUnit = 170
                },
            }
        }
    };

        await _context.Orders.AddRangeAsync(newOrders);
        await _context.SaveChangesAsync();

        // Update Product.RemainingAmount to reflect the orders
        await UpdateProductRemainingAmounts();
    }

    private async Task UpdateProductRemainingAmounts()
    {
        // For each product, calculate how much has been ordered
        var products = await _context.Products.ToListAsync();
        var orderItems = await _context.OrderItems.ToListAsync();

        foreach (var product in products)
        {
            var orderedAmount = orderItems
                .Where(oi => oi.ProductId == product.Id)
                .Sum(oi => oi.Amount);

            // Ensure RemainingAmount matches (TotalAmount - orderedAmount)
            var calculatedRemainingAmount = product.TotalAmount - orderedAmount;
            if (calculatedRemainingAmount < 0) calculatedRemainingAmount = 0;

            product.RemainingAmount = calculatedRemainingAmount;
        }

        await _context.SaveChangesAsync();
    }
}