using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.ElfSimulator.Fakers
{
    internal static class ToysFaker
    {
        private static List<string> ToyList = new List<string>()
        { "Army men","B-Daman","Digital pet","Evel Knievel Action Figure","G.I. Joe","Gumby","He-Man","Jumping Jack",
            "Kenner Star Wars action figures","Lara","Little People","Monster in My Pocket","Playmobil","Power Rangers",
            "The Smurfs merchandising","Stretch Armstrong","Teenage Mutant Ninja Turtles","Toy soldier","Transformers","Weebles",
            "Breyer Animal Creations","Filly","Ithaca Kitty","Littlest Pet Shop","My Little Pony","National Geographic Animal Jam",
            "Puppy in My Pocket","Rocking horse","Schleich","Sock monkey","Stick Horse","ZhuZhu Pets","Corgi","Cozy Coupe","Dinky",
            "Hot Wheels","Majorette","Matchbox","Power Wheels","Slot cars","Tomica","Tonka","K'Nex",
            "Lego","Lincoln Logs","Märklin","Meccano","Megablocks","Playmobil","Rasti","Rokenbok","Stickle bricks",
            "STIKFAS","Tinkertoy","Tog'l","Zaks","Zome","Cleversticks","Colorforms","Crayola Crayons",
            "Creepy Crawlers","Lite-Brite","Magna Doodle","Magnetic Poetry","Mr Potato Head","Play-Doh","Rainbow Loom","Shrinky Dinks",
            "Silly Putty","Spirograph","Stickers","African dolls","American Girl","Amish doll","Anatomically correct doll","Apple doll",
            "Art doll","Baby Alive","Ball-jointed doll","Barbie","Bisque doll","Black doll","Cabbage Patch Kids",
            "Celebrity doll","Chatty Cathy","China doll","Composition doll","Fashion doll","Frozen Charlotte","Groovy Girls",
            "Inuit doll","Japanese traditional dolls","Jumping jack (toy)","Lupita dolls","Matryoshka doll","Paper doll",
            "Parian doll","Peg wooden doll","Polly Pocket","Rag doll","Reborn doll","Shopkins","Strawberry Shortcake",
            "Tanjore doll","Teddy bear","Topsy-Turvy doll","Troll doll","Voodoo doll","Ant Farm","Lego Mindstorms",
            "Lego Mindstorms NXT","qfix robot kits","See 'n Say","Speak & Spell","LeapFrog Fridge Phonics Magnetic Letter Set",
            "Digital pet","Entertainment robot","Robot dog","Robot kit","USB toy","Atari 2600","Barrel O' Monkeys",
            "Battleship","Candy Land","Chutes and Ladders","Clue","Concentration (aka Memory)","Connect Four",
            "Dominoes","Dungeons & Dragons","Game Boy","Hungry Hungry Hippos","Life","Mad Libs","Mattel Auto Race",
            "Monopoly","Mouse Trap","Nintendo Entertainment System","Operation","PlayStation","Pong",
            "Pretty Pretty Princess","Risk","Rock 'Em Sock 'Em Robots","Scrabble","Simon","Sorry!","Toss Across",
            "Trivial Pursuit","Twister","Uno","Xbox","Jigsaw Puzzle","Mr. Potato Head","Perplexus","Puzzle","Rubik's Cube",
            "Tangrams","Chemistry set","Etch A Sketch","Jacob's ladder (toy)","Kaleidoscope","Magic 8-Ball","Sea Monkeys",
            "Spinning top","View-Master","Wooly Willy","Zoetrope","Kazoo","Moo box","Noise makers","Pop Toob",
            "Squeaky toy","Toy piano","Toy rattle","Whistle","Whirly tube"};

        private static Random rnd = new Random(DateTime.Now.Millisecond);

        public static string Toy()
        {
            var index = rnd.Next(0, ToyList.Count - 1);
            return ToyList[index];  
        }
    }
}
