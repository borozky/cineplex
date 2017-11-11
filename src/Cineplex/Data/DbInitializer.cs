using Cineplex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Data
{
    public class DbInitializer
    {
        public static async void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();



            var user = new ApplicationUser
            {
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "admin@abccc.com",
                NormalizedEmail = "admin@abccc.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "admin", NormalizedName = "admin" });
            }

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "password");
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "admin");
            }

            await context.SaveChangesAsync();






            if (context.PricingTypes.Any() == false)
            {
                context.PricingTypes.Add(new PricingType { Name = "Adult", Value = 45 });
                context.PricingTypes.Add(new PricingType { Name = "Concession", Value = 20 });
            }

            if (context.Cinemas.Any() == false)
            {
                context.Cinemas.Add(new Cinema { Location = "St. Kilda", Seats = 20 });
                context.Cinemas.Add(new Cinema { Location = "Fitzroy", Seats = 20 });
                context.Cinemas.Add(new Cinema { Location = "Melbourne CBD", Seats = 20 });
                context.Cinemas.Add(new Cinema { Location = "Sunshine", Seats = 20 });
                context.Cinemas.Add(new Cinema { Location = "Lilydate", Seats = 20 });
            }


            var images = new Image[] {
                new Image { FileName="MovieImages/light_red_scribble_hearts_on_whit.jpg"},
                new Image { FileName = "MovieImages/fast-8-tractlist.jpg" },
                new Image { FileName = "MovieImages/wwposter5.jpg" },
                new Image { FileName = "MovieImages/1412_TeenageMutantNinjaTurtles01.jpg " },
                new Image { FileName = "MovieImages/20299573_TransformersAgeofExtinction_Poster2X3.jpg" },

                new Image { FileName = "MovieImages/Predator-PosterArt.jpg" },
                new Image { FileName = "MovieImages/BattleLosAngeles-BoxArt.jpg" },
                new Image { FileName = "MovieImages/31611423_YourName.jpg" },
                new Image { FileName = "MovieImages/ETTheExtraTerrestrial-PosterArtCR.jpg" },
                new Image { FileName = "MovieImages/avengersageofultronposter.jpg" },

                new Image { FileName = "MovieImages/thor__ragnarok_poster_by_bakikayaa-daheka0.jpg" },
                new Image { FileName = "MovieImages/SKYFAL_KATOS_0002.jpg" },
                new Image { FileName = "MovieImages/The-Boss-Baby_PA_3x4.jpg" },
                new Image { FileName = "MovieImages/kong-skull-island-2017.jpg" },
                new Image { FileName = "MovieImages/HowToBeALatinLover_PA_2x3.jpg" },

                new Image { FileName = "MovieImages/Smurfs-The-Lost-Village_PA_3x4.jpg" },
                new Image { FileName = "MovieImages/FiftyShadesDarker-PosterTeaser.jpg" },
                new Image { FileName = "MovieImages/split-2016.jpg" }
            };

            if (context.Images.Any() == false)
            {
                foreach (Image image in images)
                {
                    context.Images.Add(image);
                }
            }

            var ratings = new Rating[] {
                new Rating { Abbreviation = "G", Description = "General" },
                new Rating { Abbreviation = "PG", Description = "Parental Guidance recommended" },
                new Rating { Abbreviation = "M", Description = "Recommended for mature audiences" },
                new Rating { Abbreviation = "MA15+", Description = "Not suitable for people under 15. Under 15s must be accompanied by a parent or adult guardian" },
                new Rating { Abbreviation = "R18+", Description = "Restricted to 18 and over" },
            };

            if (context.Ratings.Any() == false)
            {
                foreach (Rating rating in ratings)
                {
                    context.Ratings.Add(rating);
                }
            }

            Movie[] movies = new Movie[]
            {
                // Description from AllMovie.com
                new Movie {
                    Title ="Beauty and the Beast (2017)",
                    Rating = ratings[0],
                    Images = new List<Image>{ images[0] },
                    Description ="Christophe Gans (Brotherhood of the Wolf) directed this stylish adaptation of the classic fairy tale, in which a headstrong woman named Belle (Léa Seydoux) is forced to live with the monstrous Beast (Vincent Cassel) in his remote castle. As time passes, Belle learns to appreciate the Beast's nobler qualities, and her love proves the key to lifting his curse. André Dussollier and Eduardo Noriega co-star."
                },

                new Movie {
                    Title ="The Fate of the Furious (2017)",
                    Rating = ratings[3],
                    Images = new List<Image>{ images[1] },
                    Description ="In the eighth installment of the Fast and the Furious series, Dominic Toretto's crew are rocked to their core when they must face their most dangerous adversary yet: Dom himself (Vin Diesel), who is now working with a shadowy cyber-terrorist known only as Cipher (Charlize Theron)"
                },
                new Movie
                {
                    Title= "Wonder Woman (2017)",
                    Rating = ratings[3],
                    Images = new List<Image>{ images[2] },
                    Description = "An Amazon princess (Gal Gadot) finds her idyllic life on an island occupied only by female warriors interrupted when a pilot (Chris Pine) crash-lands nearby. After rescuing him, she learns that World War I is engulfing the planet, and vows to use her superpowers to restore peace. Directed by Patty Jenkins (Monster)."
                },
                new Movie
                {
                    Title="Teenage Mutant Ninja Turtles (2017)",
                    Rating= ratings[3],
                    Images = new List<Image>{ images[3] },
                    Description="The sinister Shredder has seized control of New York City's police and politicians, leaving his ruthless Foot Clan to spread chaos in the streets. With no prospects for salvation in sight, mutant crime-fighters Raphael, Leonardo , Donatello, and Michaelangelo leap into action. They put up a fierce fight, too, though in order to truly defeat Shredder, the heroes in a half shell will need the help of courageous reporter April O'Neil (Megan Fox) and her quick witted cameraman Vern Fenwick (Will Arnett) as well. With their help, Shredder's plan will quickly unravel, and this once-great city will rise again."
                },
                new Movie
                {
                    Title="Transformers: Age of Extinction (2014)",
                    Rating= ratings[3],
                    Images = new List<Image>{ images[4] },
                    Description= "In the devastating aftermath of the fight for humanity, an enigmatic group strives to alter the course of history as an ancient force of evil plots the destruction of mankind. In order to defeat it, Optimus Prime (voice of Peter Cullen) and the rest of the Autobots must join forces with a new, resilient band of humans who will fight an epic battle that will determine the fate of the entire human race. Mark Wahlberg and Jack Reynor co-star.",
                },
                new Movie
                {
                    Title="Schwarzenegger Predator (1987)",
                    Rating= ratings[3],
                    Images = new List<Image>{ images[5] },
                    Description = "Dutch (Arnold Schwarzenegger) has a code of honor which he will not violate, even when his life depends on it. Paradoxically, his code of honor gives him the backbone to survive as a military special forces operative when he is sent on a covert mission to rescue another group which was sent in to assist some nefarious U.S. government plan in a Latin American country. Once there, he encounters an old army buddy (Carl Weathers) who has gotten too deep in the CIA's good graces for Dutch's comfort. When he and his team go into the jungle to rescue the others, they get involved in a pitched battle with local guerillas, but they are more than capable of besting these vicious fighters. However, not long after that, they encounter signs that the equally capable men they were sent to rescue were all killed unawares and in an unusually gruesome fashion. Given their training, it should have been impossible for anyone to best all of these commando warriors. Soon, the men from Dutch's own team get picked off one by one, as they grow aware that they are up against something uncanny, not of this world, something that is hunting them for sport. Why? Because their skills make them worthy opponents for the perfectly camouflaged Predator. This carefully paced action movie was given poor reviews by many movie critics, but was sufficiently satisfying for its (largely male) audiences that a successful sequel (Predator 2) was released in 1990."
                },
                new Movie
                {
                    Title="Battle: Los Angeles (2011)",
                    Rating = ratings[3],
                    Images = new List<Image>{ images[6] },
                    Description = "A Marine platoon fights to prevent the city of Los Angeles from being overtaken by a race of highly advanced alien invaders in this epic sci-fi action thriller from director Jonathan Liebesman (The Texas Chainsaw Massacre: The Beginning) and producer Neal H. Moritz (I Am Legend, Fast & Furious). After decades of speculation about life on other planets, the people of Earth discover that extraterrestrials really do exist when destruction rains down from the stars on cities all across the globe. When the alien warships descend upon Los Angeles, however, the ferocious invaders discover that humankind won't go down without a fight as a gruff Marine staff sergeant (Aaron Eckhart) and his fearless troop of jarheads point their weapons skyward and make one last stand for the entire human race."
                },
                new Movie
                {
                    Title="Your Name (2017)",
                    Rating = ratings[0],
                    Images = new List<Image>{ images[7]},
                    Description="On the night of a beautiful meteor shower, two strangers -- a male high-school student in Tokyo, and a bored teenage girl living in rural Japan -- discover that they can switch bodies in their sleep when they share the same dream. Now, they must find each other before time runs out, all while learning lessons about what it's like to live as another person. Directed by Makoto Shinkai (Voices of a Distant Star, 5 Centimeters Per Second)."
                },
                new Movie
                {
                    Title="E.T (1982)",
                    Rating=ratings[0],
                    Images = new List<Image>{ images[8]},
                    Description = "Both a classic movie for kids and a remarkable portrait of childhood, E.T. is a sci - fi adventure that captures that strange moment in youth when the world is a place of mysterious possibilities(some wonderful, some awful), and the universe seems somehow separate from the one inhabited by grown - ups.Henry Thomas plays Elliott, a young boy living with his single mother(Dee Wallace), his older brother Michael(Robert MacNaughton), and his younger sister Gertie(Drew Barrymore).Elliott often seems lonely and out of sorts, lost in his own world.One day, while looking for something in the back yard, he senses something mysterious in the woods watching him.And he's right: an alien spacecraft on a scientific mission mistakenly left behind an aging botanist who isn't sure how to get home.Eventually Elliott puts his fears aside and makes contact with the \"little squashy guy,\" perhaps the least threatening alien invader ever to hit a movie screen.As Elliott tries to keep the alien under wraps and help him figure out a way to get home, he discovers that the creature can communicate with him telepathically.Soon they begin to learn from each other, and Elliott becomes braver and less threatened by life.E.T.rigs up a communication device from junk he finds around the house, but no one knows if he'll be rescued before a group of government scientists gets hold of him. In 2002, Steven Spielberg re-released E.T. The Extra-Terrestrial in a revised edition, with several deleted scenes restored and digitally refurbished special effects."
                },
                new Movie
                {
                    Title="Avengers: Age of Ultron (2015)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[9]},
                    Description = "This sequel to the smash-hit comic-book epic The Avengers finds the iconic superhero team dealing with a threat of their own making: a sentient robot called Ultron (voice of James Spader), who was originally designed as part of a peacekeeping program. Since the events of the last film, Captain America (Chris Evans), Iron Man (Robert Downey Jr.), the Hulk (Mark Ruffalo), Thor (Chris Hemsworth), Hawkeye (Jeremy Renner), and Black Widow (Scarlett Johansson) have been working to take down various cells of a secret society of villains known as HYDRA. Their zeal to make the world a better, safer place inspires Tony Stark, genius billionaire and alter ego of Iron Man, to create Ultron in order to respond to additional threats that the Avengers aren't able to handle. Ultron, unfortunately, takes this directive way too seriously -- he believes that world peace can only be achieved by exterminating humanity, and he'll stop at nothing to accomplish this goal. The battle between the Avengers and Ultron is further complicated by the appearance of superpowered siblings Quicksilver (Aaron Taylor-Johnson) and Scarlet Witch (Elizabeth Olsen), who ally themselves with the homicidal android. Samuel L. Jackson and Cobie Smulders co-star as, respectively, S.H.I.E.L.D. operatives Nick Fury and Maria Hill. Joss Whedon, writer and director of the previous Avengers movie, returns in both capacities here."
                },
                new Movie
                {
                    Title="Thor Ragnarok (2017)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[10]},
                    Description = "Chris Hemsworth returns as everyone's favorite hammer-wielding hero from Asgard in the next chapter of this Marvel Studios film directed by Taika Waititi (Hunt for the Wilderpeople)."
                },
                new Movie
                {
                    Title="Skyfall (2012)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[11]},
                    Description = "007 (Daniel Craig) becomes M's only ally as MI6 comes under attack, and a mysterious new villain emerges with a diabolical plan. James Bond's latest mission has gone horribly awry, resulting in the exposure of several undercover agents, and an all-out attack on M16. Meanwhile, as M (Judi Dench) plans to relocate the agency, emerging Chairman of the Intelligence and Security Committee Mallory (Ralph Fiennes) raises concerns about her competence while attempting to usurp her position, and Q (Ben Whishaw) becomes a crucial ally. Now the only person who can restore M's reputation is 007. Operating in the dark with only field agent Eve (Naomie Harris) to guide him, the world's top secret agent works to root out an enigmatic criminal mastermind named Silva (Javier Bardem) as a major storm brews on the horizon. Albert Finney also stars in the 23rd installment of the long-running spy series. The film was directed by Sam Mendes (American Beauty, Revolutionary Road) and shot by acclaimed cinematographer Roger Deakins (True Grit, The Reader, The Assassination of Jesse James by the Coward Robert Ford)."
                },
                new Movie
                {
                    Title="The Boss Baby (2017)",
                    Rating=ratings[1],
                    Images = new List<Image>{ images[12]},
                    Description="A seven-year-old boy (voice of Miles Christopher Bakshi) has his life turned upside down by his new brother, an infant known as the Boss Baby (Alec Baldwin) who wears a suit and has the brusque manner of a businessman. However, the two are forced to get past their initial antagonism when they must work together on a mission of espionage involving the rivalry between babies and puppies. Steve Buscemi, Lisa Kudrow, Jimmy Kimmel, and Tobey Maguire also lend their voices to this animated comedy, which is based on a picture book by Marla Frazee."
                },
                new Movie
                {
                    Title="Kong: Skull Island (2017)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[13]},
                    Description = "A secret government organization mounts an expedition to Skull Island, an uncharted territory in the Pacific. Led by an explorer (John Goodman) and a lieutenant colonel (Samuel L. Jackson), the group recruit a disillusioned soldier (Tom Hiddleston) and a photojournalist (Brie Larson) to investigate the island's peculiar seismic activity. But once there, they discover that Skull Island is home to a gigantic ape called King Kong, and find themselves caught up in an ongoing war between the beast and the area's indigenous predators. Jordan Vogt-Roberts directed this reboot of the classic monster franchise."
                },
                new Movie
                {
                    Title="How to Be a Latin Lover (2017)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[14]},
                    Description = "A past-his-prime Latin lover (Eugenio Derbez) moves in with his sister (Salma Hayek) and her son (Raphael Alejandro) after his wife leaves him for a younger man. As he teaches his nephew how to play the game of seduction, he vows to return to his lifestyle of being a kept man for an older, wealthy woman. Michael Cera, Rob Lowe, Kristen Bell, Raquel Welch, Rob Corddry, and Rob Riggle co-star in this comedy directed by Ken Marino."
                },
                new Movie
                {
                    Title="Smurfs: The Lost Village (2017)",
                    Rating=ratings[2],
                    Images = new List<Image>{ images[15]},
                    Description = "The little blue homunculi created by Belgian cartoonist Peyo return for another adventure, this time involving a quest through the Forbidden Forest to discover a long-hidden village hinted at in a treasure map. Smurfette (voiced by Demi Lovato), Brainy (Danny Pudi), Clumsy (Jack McBrayer), and Hefty (Joe Manganiello) must evade the clutches of the evil wizard Gargamel (Rainn Wilson) as they embark on their journey to find another community of Smurfs. Julia Roberts, Ariel Winter, Michelle Rodriguez, Ellie Kemper, and Mandy Patinkin also lend their voices to this animated fantasy."
                },
                new Movie
                {
                    Title="Fifty Shades Darker (2017)",
                    Rating=ratings[2],
                    Images = new List<Image>{ images[16]},
                    Description = "This sequel to Fifty Shades of Grey adapts the second installment in E.L. James' best-selling trilogy of novels, which chronicle the torrid love affair between the demure Anatasia Steele (Dakota Johnson) and possessive businessman Christian Grey (Jamie Dornan). This time around, Steele and Grey's attempts to build a more trusting relationship are threatened by several of his past lovers.James Foley directed this erotic drama, which co-stars Kim Basinger, Rita Ora, Eric Johnson, Bella Heathcote, and Luke Grimes."
                },
                new Movie
                {
                    Title="Split (2016)",
                    Rating=ratings[3],
                    Images = new List<Image>{ images[17]},
                    Description= "An outing takes a sinister turn for three teenage friends (Anya Taylor-Joy, Haley Lu Richardson, and Jessica Sula) when they are kidnapped by a ruthless stranger (James McAvoy) and imprisoned in his basement. They soon learn that their captor has multiple-personality disorder, forcing them to plot their escape without ever knowing which of his 23 personas -- young or old, male or female, benign or monstrous -- they will confront on the way out. Written and directed by M. Night Shyamalan."
                }
                // additional movies here

            };

            if (context.Movies.Any() == false)
            {
                foreach (Movie m in movies)
                {
                    context.Movies.Add(m);
                }
            }

            context.SaveChanges();

        }
    }
}
