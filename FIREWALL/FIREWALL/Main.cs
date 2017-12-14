using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace FIREWALL
{
    enum GameState { IntroText, MainMenu, InGame, SoloPWin, SoloCWin, CompP1Win, CompP2Win, About, Code, CodeDesc, Send, Credits };
    enum Collide { None, OwnPaddle, OtherPaddle, OwnGoal, OtherGoal }
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D render;

        GameState state;
        MouseState mouse, prevMouse;

        Vector2Int res;

        Texture2D bg, pixel, ellipse, grid, cursor, tax;
        Animation codebg;
        Paddle p1, p2;
        Ball b1, b2;
        List<List<Brick>> brickWall;
        bool isComp;

        SpriteFont terminator, alienEncounters;
        Color red, shield_blue, blue;

        float theta;
        Random rng = new Random();

        Text introText,
            menutitle, menub1, menub2, menub3, menub4, menub5,
            back, quit,
            aboutDesc, aboutPan1, aboutPan2,
            blueWin, redWin, code,
            failTitle, failSubtitle, failTAX,
            send, sent, credits;
        List<Text> codesList, codeButtons, codeTitle, codeDesc;
        List<bool> isUnlocked;
        List<Text> unlockables;
        bool isAllUnlocked;
        int codeCursor;

        Song allcodes, cyberdyne, desertsuite, failcode, gunshop, helicopter, message, sarahsdream, steelmill, trustme, uvb;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            res = new Vector2Int(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Window.IsBorderless = true;

            state = GameState.IntroText;

            red = new Color(232, 44, 12);
            shield_blue = new Color(68, 154, 232);
            blue = new Color(4, 130, 232);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            render = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            mouse = new MouseState();
            prevMouse = new MouseState();

            pixel = Content.Load<Texture2D>("assets/pixel");
            ellipse = Content.Load<Texture2D>("assets/circle");
            grid = Content.Load<Texture2D>("assets/grid");
            bg = Content.Load<Texture2D>("assets/introbg");
            cursor = Content.Load<Texture2D>("assets/cursor");
            tax = Content.Load<Texture2D>("assets/tax");

            List<Texture2D> frames = new List<Texture2D>();
            for (int i = 0; i < 44; i++)
            {
                frames.Add(Content.Load<Texture2D>("codebg/" + i));
            }
            codebg = new Animation(frames, new Rectangle(0, 0, res.X, res.Y), 20);


            terminator = Content.Load<SpriteFont>("fonts/Terminator");
            alienEncounters = Content.Load<SpriteFont>("fonts/AlienEncounters");

            p1 = new Paddle(new Rectangle(0, res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, shield_blue, 0);
            p2 = new Paddle(new Rectangle(res.X - res.pixelWidth(.0185), res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, red, 1);

            b1 = new Ball(new Rectangle(res.pixelWidth(.4375) - res.pixelHeight(.0555) - 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, shield_blue);
            b2 = new Ball(new Rectangle(res.pixelWidth(.5625) + 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, red);
            theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
            b1.sendOff(new Vector2Int((int)Math.Round(-(res.pixelWidth(.0052)) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
            theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
            b2.sendOff(new Vector2Int((int)Math.Round(res.pixelWidth(.0052) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));

            brickWall = new List<List<Brick>>();
            for (int i = 0; i < 6; i++)
            {
                List<Brick> brickRow = new List<Brick>
                {
                    new Brick(new Rectangle(res.pixelWidth(.5) - 2 * res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                    new Brick(new Rectangle(res.pixelWidth(.5) - res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                    new Brick(new Rectangle(res.pixelWidth(.5), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                    new Brick(new Rectangle(res.pixelWidth(.5) + res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                };
                brickWall.Add(brickRow);
            }
            introText = new Text(terminator, new Rectangle(res.X / 2 - res.pixelWidth(.8976) / 2, res.Y / 2 - res.pixelHeight(.8517) / 2, res.pixelWidth(.8976), res.pixelHeight(.8517)), "  IT IS THE YEAR 2255. HUMANITY HAS COME UNDER ATTACK BY A RACE KNOWN ONLY AS ÒTHE ORDERÓ. WAR RAGES THROUGHOUT THE GALAXY AS THE ORDER SEEKS TO ASSERT ITSELF AS AN INTERGALACTIC EMPIRE. THE FORCES OF HUMANITY ARE GRAVELY OUTMATCHED, UNABLE TO SUPPORT A LOSING BATTLE. IN A DESPERATE ATTEMPT TO GAIN AN UPPER HAND, THE GALACTIC COUNCIL OF HUMAN SYSTEMS HAS DISPATCHED POCKETS OF SPECIALLY TRAINED OPERATIVES TO THE FRONT LINES. UPON THEIR CAPTURE BY THE ORDER, THESE OPERATIVES ARE PLACED IN ÒHOLDING DISTRICTSÓ AS PRISONERS. HERE, BEHIND ENEMY LINES, THESE OPERATIVES GATHER INTEL AND PERFORM A COMBINATION OF PRECISION AND GUERRILLA STRIKES AGAINST THE ORDER.\n\n  WHEN NOT ENGAGING THE ENEMY, MANY PRISONERS FEEL IT WISE TO KEEP THEIR WITS SHARPENED. REX, THE LEADER OF CELL 13, HAS DEVISED A SIMPLE GAME TO TEST REFLEXES AND HANDÐEYE COORDINATION, ASIDE FROM A WAY TO PASS TIME IN THE DISTRICT, THIS GAME ALSO SERVES AS A MEANS OF PASSING CODED MESSAGES BETWEEN CELLS.\n\n  THIS GAME OFFERS TWO METHODS OF PLAY. YOU CAN TEST YOUR SKILL AGAINST ANOTHER CELLMATE IN FRIENDLY COMPETITION, OR YOU CAN FACE OFF AGAINST THE DISTRICT'S CYBERÐSECURITY. COMPETITIVE GAMES OFFER NO REWARD, BUT BATTLING THE SECURITY WILL ALLOW MESSAGES THROUGH OUR CHANNELS. JUST BE CAUTIOUS OF THE T.A.X. ...", red, true, 80);
            introText.optimize();

            menutitle = new Text(terminator, new Rectangle(res.pixelWidth(.1643), res.pixelHeight(.45625), res.pixelWidth(.6725), res.pixelHeight(.1021)), "FIRE WALL", red, false, 1);
            menutitle.optimizeLine();

            menub1 = new Text(terminator, new Rectangle(res.pixelWidth(.3567), res.pixelHeight(.705) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), "SOLO", Color.White, false, 1);
            menub1.optimizeLine();

            menub2 = new Text(terminator, new Rectangle(res.pixelWidth(.5253), res.pixelHeight(.705) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), "COMPETITIVE", Color.White, false, 1);
            menub2.optimizeLine();

            menub3 = new Text(terminator, new Rectangle(res.pixelWidth(.3567), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), "ABOUT", Color.White, false, 1);
            menub3.optimizeLine();

            menub4 = new Text(terminator, new Rectangle(res.pixelWidth(.5253), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), "CODES", Color.White, false, 1);
            menub4.optimizeLine();

            menub5 = new Text(terminator, new Rectangle(res.pixelWidth(.8796), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), "CREDITS", Color.White, false, 1);
            menub5.optimizeLine();

            back = new Text(terminator, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), "BACK", Color.White, false, 1);
            back.optimizeLine();

            quit = new Text(terminator, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), "QUIT", Color.White, false, 1);
            quit.optimizeLine();

            aboutDesc = new Text(terminator, new Rectangle(res.pixelWidth(.015), res.pixelHeight(.0533), res.pixelWidth(.9699), res.pixelHeight(.2667)), "ÒFIRE WALLÓ IS ESSENTIALLY A GAME OF PONG WITH COLUMNS OF BREAKOUT BRICKS JAMMED IN THE MIDDLE. THE RULES ARE SIMPLE: BREAK THROUGH THE WALL AND SCORE ON YOUR OPPONENT. DON'T WORRY, THERE'S NO PENALTY FOR MISSING YOUR OWN BALL.", red, false, 1);
            aboutDesc.optimize();

            aboutPan1 = new Text(terminator, new Rectangle(res.pixelWidth(.015), res.pixelHeight(.4267), res.pixelWidth(.4221), res.pixelHeight(.3933)), "  CONTROLS\nBLUE\nÐ WÑUP\nÐ SÑDOWN\n\nRED\n(COMPETITIVE ONLY)\nÐ UPÐARROWÑUP\nÐDOWNÐARROWÑDOWN", red, false, 1);
            aboutPan1.optimize();

            aboutPan2 = new Text(terminator, new Rectangle(res.pixelWidth(.4465), res.pixelHeight(.4267), res.pixelWidth(.5084), res.pixelHeight(.3933)), "  ADDITIONAL TIPS\nÐ BRICKS BROKEN DO NOT AFFECT SCORE\nÐ LIKE PONG, ONLY GOALS COUNT\nÐ WIN SOLO GAMES TO RECEIVE MESSAGES FROM OTHER CELLS\nÐ FIND ALL MESSAGES TO COMPLETE THE STORY\nÐ BUT BE CAREFUL, SOMEONE'S WATCHING...", red, false, 1);
            aboutPan2.optimize();

            blueWin = new Text(terminator, new Rectangle(res.pixelWidth(.23), res.pixelHeight(.1539), res.pixelWidth(.5375), res.pixelHeight(.0432)), "BLUE VICTORY!", red, false, 1);
            blueWin.optimizeLine();

            redWin = new Text(terminator, new Rectangle(res.pixelWidth(.23), res.pixelHeight(.1539), res.pixelWidth(.5375), res.pixelHeight(.0432)), "RED VICTORY!", red, false, 1);
            redWin.optimizeLine();

            code = new Text(terminator, new Rectangle(res.pixelWidth(.2438), res.pixelHeight(.5197), res.pixelWidth(.5075), res.pixelHeight(.0432)), "CODE RECEIVED:", red, false, 1);
            code.optimizeLine();

            failTitle = new Text(terminator, new Rectangle(res.pixelWidth(.23), res.pixelHeight(.1539), res.pixelWidth(.5375), res.pixelHeight(.0432)), "CONNECTION INTERRUPTED", red, false, 1);
            failTitle.optimizeLine();

            failSubtitle = new Text(terminator, new Rectangle(res.pixelWidth(.2438), res.pixelHeight(.5197), res.pixelWidth(.5075), res.pixelHeight(.0432)), "UPLINK TERMINATED", red, false, 1);
            failSubtitle.optimizeLine();

            failTAX = new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "TAX IS WATCHING...", red, false, 1);
            failTAX.optimizeLine();

            codesList = new List<Text>()
            {
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "LISMITE", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "TROASERVATY", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "OMEGAFEAR", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "MOONHEAD", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "HEASMS", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "AUTODANG", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "APHYDRON", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.1788), res.pixelHeight(.7092), res.pixelWidth(.6438), res.pixelHeight(.0432)), "934018", red, false, 1)
            };
            codeButtons = new List<Text>()
            {
                new Text(terminator, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.1557) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "LISMITE", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.1557) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "TROASERVATY", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.3438) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "OMEGAFEAR", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.3438) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "MOONHEAD", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.5318) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "HEASMS", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.5318) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "AUTODANG", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.7198) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "APHYDRON", Color.White, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.7198) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), "934018", Color.White, false, 1)
            };
            codeTitle = new List<Text>()
            {
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "UNKEMPT BOOKS", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "OH BAO LAN'G", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "A LIGHT BEYOND THE RIVER", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "MACHINE WITHIN THE GHOST", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "THE LEDGE", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "STEAM CANNON", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "ACCORDION LUNG", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0333)), "FAILURE", red, false, 1)
            };
            codeDesc = new List<Text>()
            {
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "IN AMONG THE STACKS OF BOXES,\nWHERE DARK AND TWISTED SHAPES CONVERGE\nTO HIDE THE MIND ITS TOXINS LETHAL;\nHERE IS WHERE FEAR IS OBSERVED.\n\nDUST AND COBWEBS, BITS OF GLASS,\nYELLOWED PAGES REEK OF MOLD,\nRUBBER SEALS THAT SMELL OF GAS;\nHERE IS KEPT THE HATE OF OLD.\n\nBROKEN LIGHTBULBS, STICKY FLOOR,\nPOSTERS CURLING OFF THE DOOR,\nFIGMENTS, FRAGMENTS, NEVERMORE,\nALL THIS WRETCHEDNESS ABHOR.\n\nSOME THINGS ARE BEST LEFT FORGOTTEN,\nTHROWN IN TRASH BAGS, DISCARD PILES\nTOPPLE, BURSTING, RENT, DISGORGING,\nRECOILS THE MIND AS IT REVILES.\n\nFILTH AND WASTE, SLING AND SMEAR,\nDIG THE TRENCHES DEEP AND WIDE,\nEVEN NOW THE PAIN CONDENSES,\nDARKNESS, HUNGRY, BEGS A TASTE.\n\nHARK, FOOTFALLS, SOMEONE APPROACHES,\nIN BILEÑTHICK DARKNESS CANNOT SEE;\nFLICKS A LIGHTSWITCH ROACHES SCATTERÑ\nCOME AND SEE JUST WHAT'S THE MATTER.\n\nWHO IS THIS REPULSIVE CREATURE,\nBRINGING LIGHT WHERE DARKNESS DWELLS?\nÒHERETIC!Ó THE MASSLESS SHRIEK,\nGROPING, CLAWING, TEAR THE SOUL.", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "WELCOME BACK TO THE ATTACK\nON THE PLANET OF THE MISTS,\nOH, BAO LAN'G, OH, HOW LONG\nMUST THIS ENDLESS WAR PERSIST,\nWHILE GOOD MEN FIGHT AND DIE\nFOR THE ORDER TO DENY\nTHEIR CONTROL OF OUR LIVES\nAND THEIR INFLUENCE RESIST?\n\nALIEN:\n\nVOLNEK STA OP KREVA\nHUL OP SORIS YD OP FZA,\nROF BAOÐLANG, ROF WOD QOG\nXOL TYS NORTVEK KRYV GNZOG,\nBROF KAV SNAM KRIV OL KLYV\nUL OP DRULEK OV CEDYV\nREIL KRONAG YD OS KZOV\nOL REIL GNELDPINAK KZENDROV?", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "ON A NIGHT SO DEEP IN WINTER\nUNDER CLOUDED SNOWY SKIES\nI RETIRED TO MY BEDROOM\nAS TO REST MY WEARY EYES.\n\nIN RESTLESS SLEEP I LINGERED\nCHASING NIGHTMARES SO DESPISED\nWHEN MY INSIDES GAVE A QUIVER\nAND I FELT THE NEED TO RISE.\n\nAS I DID I SAW A GLINTING\nFROM THE CORNER OF MY EYE\nA REFLECTION ON THE WINDOW\nWHAT COULD IT BE SO LATE AT NIGHT?\n\nI OPENED UP THE WINDOW\nAND WITHIN THE DARKNESS SPIED\nA LIGHT BEYOND THE RIVER\nWHAT WAS ON THE OTHER SIDE?\n\nONE IF BY LAND, TWO IF BY SEA,\nTHREE IF BY AIR, FOUR BY DECREE,\nFIVE IF BY WILL, SIX IF BY TRICK,\nSEVEN BY MERITS, EIGHT IF THEY'RE QUICK,\nNINE IF UNARMED, TEN IF THEY CARRY,\nELEVEN FROM HELL, AND TWELVE TO BE WARY...", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "SEARCHING. ALWAYS SEARCHING.\nAIMLESSLY WANDERING.\nDOWN DARK CORRIDORS,\nTHROUGH EMPTY ROOMS,\nPAST DEAD ENDS.\n\nALWAYS SEARCHING.\nBEHIND CLOSED DOORS,\nUP LIFTS,\nIN STORAGE,\nIN CORES.\n\nALWAYS SEARCHING.\nTHROUGH CHAMBERS GRAND,\nAND RUBBISH HEAPS,\nBEYOND WINDOWS GLEAMING\nDELVING INTO THE HEART OF THE UNIVERSE.\n\nALWAYS SEARCHING.\nUNFETTERED BY SLEEP,\nUNBOUND BY GRAVITY,\nUNHURRIED BY TIME,\nUNSEEN BY SPACE.\nUNKNOWN BY EXISTENCE...\n\nALWAYS SEARCHING, NEVER FINDING, MIND BINDING, GEARS GRINDING, EARS WHINING, EYES WATERING TEETH CHATTERING SKIN TINGLING NOSE STINGING HEART POUNDING BREATH FALTERING... BODY SHIVERING...\n\nSOUL WITHERING.\n\nALWAYS SEARCHING,\nFOR THE ANSWER,\nFOR THE QUESTION,\nFOR THE LASTÐKNOWN SIGNAL,\nFOR ANYTHING.\n\nTHE MACHINE WITHIN THE GHOST, THE ETERNAL CURSE.", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "YOU FIND YOURSELF TRYING TO ESCAPE, TRYING TO GET AWAY, BUT THAT THING KEEPS CHASING YOU. YOU'VE GOTTEN A GOOD WAYS AHEAD OF IT, BUT YOU KNOW IT'LL FIND YOU. AS YOU DASH ACROSS THE ROCKY GROUND, YOU SUDDENLY LOSE YOUR FOOTING AND FIND YOURSELF ON YOUR STOMACH, STARING DOWN OVER A HIGH LEDGE. YOU FLIP ONTO YOUR BACK AND SKITTER BACK AWAY FROM IT. YOU DON'T KNOW WHAT'S DOWN THERE, ALL YOU COULD SEE WAS THE FOG. YOU CAN HEAR THE THING COMING, YOU KNOW YOU HAVE TO DO SOMETHING. YOU CRAWL BACK TO THE EDGE, KNEES SHAKING, AND STARE ONCE MORE DOWN INTO THE ABYSS. IT'S SIX OF ONE, A HALFÐDOZEN OF THE OTHER. SO, DO YOU STAY AND WAIT FOR THE THING TO BRING YOUR DOOM? OR DO YOU TAKE A CHANCE AND JUMP?", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "ONCE THE SEQUENCE HAS BEGUN, IT CAN'T BE STOPPED. THE PLATE COVERING THE CONTAINING AREA LIFTS AND SLIDES AWAY, THE BODY LIFTS INTO THE SUNLIGHT, THE LONG BARREL EXTENDS, TELESCOPING OUT FROM THE BOX. IT DOESN'T TAKE MUCH TO START THE SEQUENCE. JUST A FEW WORDS, JUST A FEW SIMPLE WORDS. SOMETIMES NOT EVEN THAT. BUT WHAT IS DONE CANNOT BE UNDONE. WITH EACH NEXT WORD, THE SYSTEM BOILS THE WATER, COMPRESSES THE STEAM, FILLS COUNTLESS CANISTERS. THE CANISTERS ARE SENT TO THE BOX, AND FROM THERE THE BELT FEEDS THEM INTO THE LAUNCHING TUBE. ONCE A CANISTER IS LOADED INTO THE LAUNCHING TUBE IT IS SEALED, AND THE PRESSURE IS THEN SUDDENLY RELEASED. THE CANISTER IS LAUNCHED. THE FIRST SHOT HAS BEEN FIRED. THE CANISTER ARCS UP, UP, UP, INTO THE SUNLIGHT, LEAVING A TRAIL OF VAPOR BEHIND IT, GROWING EVER FAINTER AS IT SAILS AWAY. THE TARGETING SYSTEM ASSURES THAT THE CANISTER WILL ARRIVE. ...BUT THE TARGETING SYSTEM IS FLAWED. THE CANISTER SAILS FAR PAST THE INTENDED TARGET, AND THE SEQUENCE REPEATS ITSELF. THE SECOND CANISTER FALLS SHORT. THE THIRD CANISTER GOES WIDE. THE FOURTH CANISTER GETS CLOSE...BUT BY THEN IT'S TOO LATE. THE RETALIATION HAS BEGUN. THERE IS NO TIME FOR RECALCULATION, NO TIME TO RECALIBRATE THE TARGETING SYSTEM. EVEN IF THERE WAS TIME...IT IS IMPOSSIBLE TO RECALIBRATE THE TARGETING SYSTEM.", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "HE AMBLES SLOWLY AROUND THE WORKSHOP, WHEEZING ALL THE WHILE. THE KEYS CLICK, AND A TUNELESS SQUAWK ESCAPES HIS THROAT. EVER SINCE THE REPLACEMENT, HE'S FELT...DIFFERENT. HE'S BEEN MUCH HAPPIER, WHEN HE CAN WORK. BUT WHEN HE'S FORCED TO BE SHUT DOWN, LOCKED AWAY AGAIN, HE DOESN'T KNOW IF THEY'LL HAVE ANY NEED OF HIM ANY LONGER. HE'S OLD ENOUGH AS IT IS. HE'S BEEN ALTERED SO MANY TIMES HE CAN'T EVEN REMEMBER WHAT HIS PLATING NORMALLY LOOKED LIKE. WAS IT A SHINING WHITE? IMPERVIOUS TO THE MOST DEADLY BLASTS? CONCEALING THE INNER WORKINGS OF A MACHINE ONLY GODS COULD HAVE CREATED? NOW HE'S JUST BITS OF OTHERS, COBBLED TOGETHER TO KEEP HIM FUNCTIONING. THIS ACCORDIONÐLUNG IS THE LATEST. HE KNOWS HIS TIME MAY BE COUNTING DOWN, BUT AT LEAST HE STILL HAS SOME LEFT. HE WHEEZES HAPPILY, KNOWING THAT SINCE THE CORPORATION IS SO LOW ON WORKERS THAT HE'S STILL GUARANTEED TIME. HE SAUNTERS ON, SHAKEN OUT OF HIS MOMENTARY SADNESS BY THE CLATTERING OF THE MACHINES AROUND HIM AND THE DISTANT BOOM OF THE PRESSES OFF IN THE MAIN BUILDING. IT'S GOING TO BE A COLD NIGHT...", red, false, 1),
                new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.11415), res.pixelWidth(.8976), res.pixelHeight(.76)), "934018", red, false, 1)
            };
            unlockables = new List<Text>();
            isUnlocked = new List<bool>();
            for(int i = 0; i < codesList.Count; i++)
            {
                codesList[i].optimizeLine();
                codeButtons[i].optimizeLine();
                codeTitle[i].optimizeLine();
                codeDesc[i].optimize();
                unlockables.Add(codesList[i]);
                isUnlocked.Add(false);
            }
            isAllUnlocked = false;
            codeCursor = 0;

            send = new Text(terminator, new Rectangle(res.pixelWidth(.8796), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), "SEND", Color.White, false, 1);
            send.optimizeLine();

            sent = new Text(terminator, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.0666)), "ALL MESSAGES RECEIVED,\nSTANDBY FOR FURTHER INSTRUCTIONS...", red, true, 80);
            sent.optimize();

            credits = new Text(terminator, new Rectangle(res.pixelWidth(.0687), res.pixelHeight(.1464), res.pixelWidth(.8626), res.pixelHeight(.7072)), "CREDITS \nMUSIC \n\nTERMINATOR SOUNDTRACK (BRAD FIEDEL) \nÐ GUN SHOP/REESE IN ALLEY \n\nTERMINATOR 2 SOUNDTRACK (BRAD FIEDEL) \nÐ OUR GANG GOES TO CYBERDYNE \nÐ TRUST ME \nÐ SWAT TEAM ATTACKS \nÐ HELICOPTER CHASE \nÐ INTO THE STEEL MILL \nÐ DESERT SUITE \nÐ SARAH'S DREAM \n\nBLACK MESA SOUNDTRACK (JOEL NIELSON) \nÐ BLAST PIT \n\nTHE THING SOUNDTRACK (ENNIO MORRICONE) \nÐ DESPAIR \nÐ HUMANITY PT. 2", red, false, 1);
            credits.optimize();

            allcodes = Content.Load<Song>("audio/allcodes");
            cyberdyne = Content.Load<Song>("audio/cyberdyne");
            desertsuite = Content.Load<Song>("audio/desertsuite");
            failcode = Content.Load<Song>("audio/failcode");
            gunshop = Content.Load<Song>("audio/gunshop");
            helicopter = Content.Load<Song>("audio/helicopter");
            message = Content.Load<Song>("audio/message");
            sarahsdream = Content.Load<Song>("audio/sarahsdream");
            steelmill = Content.Load<Song>("audio/steelmill");
            trustme = Content.Load<Song>("audio/trustme");
            uvb = Content.Load<Song>("audio/uvb");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(cyberdyne);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && state != GameState.Send)
                Exit();

            mouse = Mouse.GetState();

            switch (state)
            {
                case GameState.IntroText:
                    if (!introText.isTyping)
                        introText.isTyping = true;
                    introText.Update(gameTime);
                    // MAKE THIS: if (introText.isTyped && mouse.LeftButton == ButtonState.Pressed)
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        MediaPlayer.Play(trustme);
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.MainMenu:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.3455), res.pixelHeight(.705), res.pixelWidth(.1404), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(gunshop);
                        isComp = false;
                        state = GameState.InGame;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5141), res.pixelHeight(.705), res.pixelWidth(.1404), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(gunshop); //ASK GATH
                        isComp = true;
                        state = GameState.InGame;
                    }
                    if(new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.3455), res.pixelHeight(.875), res.pixelWidth(.1404), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(sarahsdream);
                        state = GameState.About;
                    }
                    if(new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5141), res.pixelHeight(.875), res.pixelWidth(.1404), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(uvb);
                        codebg.toggle();
                        state = GameState.Code;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.8684), res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(sarahsdream);
                        bg = Content.Load<Texture2D>("assets/introbg");
                        state = GameState.Credits;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        Exit();
                    break;
                case GameState.InGame:
                    int wallHit = b1.Update(gameTime, res);
                    if (wallHit == 0)
                    {
                        b1 = new Ball(new Rectangle(res.pixelWidth(.4375) - res.pixelHeight(.0555) - 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, shield_blue);
                        theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                        b1.sendOff(new Vector2Int((int)Math.Round(-(res.pixelWidth(.0052)) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                    }
                    else if (wallHit == 1)
                    {
                        p1.score++;
                        if (p1.score == 4)
                            MediaPlayer.Play(helicopter);
                        b1 = new Ball(new Rectangle(res.pixelWidth(.4375) - res.pixelHeight(.0555) - 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, shield_blue);
                        theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                        b1.sendOff(new Vector2Int((int)Math.Round(-(res.pixelWidth(.0052)) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                        b2 = new Ball(new Rectangle(res.pixelWidth(.5625) + 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, red);
                        theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                        b2.sendOff(new Vector2Int((int)Math.Round(res.pixelWidth(.0052) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                        brickWall = new List<List<Brick>>();
                        for (int i = 0; i < 6; i++)
                        {
                            List<Brick> brickRow = new List<Brick>
                            {
                                new Brick(new Rectangle(res.pixelWidth(.5) - 2 * res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                                new Brick(new Rectangle(res.pixelWidth(.5) - res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5) + res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                            };
                            brickWall.Add(brickRow);
                        }
                    }
                    else
                    {
                        wallHit = b2.Update(gameTime, res);
                        if (wallHit == 0)
                        {
                            p2.score++;
                            if (p2.score == 4)
                                MediaPlayer.Play(helicopter);
                            b1 = new Ball(new Rectangle(res.pixelWidth(.4375) - res.pixelHeight(.0555) - 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, shield_blue);
                            theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                            b1.sendOff(new Vector2Int((int)Math.Round(-(res.pixelWidth(.0052)) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                            b2 = new Ball(new Rectangle(res.pixelWidth(.5625) + 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, red);
                            theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                            b2.sendOff(new Vector2Int((int)Math.Round(res.pixelWidth(.0052) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                            for (int i = 0; i < 6; i++)
                            {
                                List<Brick> brickRow = new List<Brick>
                            {
                                new Brick(new Rectangle(res.pixelWidth(.5) - 2 * res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                                new Brick(new Rectangle(res.pixelWidth(.5) - res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5) + res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                            };
                                brickWall.Add(brickRow);
                            }
                        }
                        else if (wallHit == 1)
                        {
                            b2 = new Ball(new Rectangle(res.pixelWidth(.5625) + 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, red);
                            theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                            b2.sendOff(new Vector2Int((int)Math.Round(res.pixelWidth(.0052) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                        }
                    }
                    p1.Update(gameTime, res);
                    if (isComp)
                        p2.Update(gameTime, res);
                    else
                        p2.Update(gameTime, res, new List<Ball>() { b1, b2 });

                    b1.collide(p1);
                    b1.collide(p2);
                    b2.collide(p1);
                    b2.collide(p2);
                    
                    for(int i = 0; i < brickWall.Count; i++)
                        for(int j = 0; j < brickWall[i].Count; j++)
                            if (b1.collide(brickWall[i][j]) || b2.collide(brickWall[i][j]))
                                brickWall[i].RemoveAt(j--);

                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                        p1.score = 4;
                    if(p1.score >= 5 || p2.score >= 5)
                    {
                        b1 = new Ball(new Rectangle(res.pixelWidth(.4375) - res.pixelHeight(.0555) - 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, shield_blue);
                        theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                        b1.sendOff(new Vector2Int((int)Math.Round(-(res.pixelWidth(.0052)) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                        b2 = new Ball(new Rectangle(res.pixelWidth(.5625) + 20, res.pixelHeight(.47225), res.pixelHeight(.0555), res.pixelHeight(.0555)), ellipse, red);
                        theta = MathHelper.ToRadians((float)(rng.NextDouble()) * 45 + 45);
                        b2.sendOff(new Vector2Int((int)Math.Round(res.pixelWidth(.0052) * Math.Sin(theta)), (int)((new Random().Next(2) * 2 - 1) * Math.Round(res.pixelWidth(.0052) * Math.Cos(theta)))));
                        brickWall = new List<List<Brick>>();
                        for (int i = 0; i < 6; i++)
                        {
                            List<Brick> brickRow = new List<Brick>
                            {
                                new Brick(new Rectangle(res.pixelWidth(.5) - 2 * res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                                new Brick(new Rectangle(res.pixelWidth(.5) - res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, red),
                                new Brick(new Rectangle(res.pixelWidth(.5) + res.pixelWidth(.03125), i * (res.Y / 6), res.pixelWidth(.03125), res.Y / 6), pixel, shield_blue),
                            };
                            brickWall.Add(brickRow);
                        }

                        if (p1.score >= 5 && !isComp)
                        {
                            p1 = new Paddle(new Rectangle(0, res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, shield_blue, 0);
                            p2 = new Paddle(new Rectangle(res.X - res.pixelWidth(.0185), res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, red, 1);
                            if (unlockables.Count > 0)
                            {
                                int r = rng.Next(0, unlockables.Count);
                                for (int i = 0; i < codesList.Count; i++)
                                    if (codesList[i] == unlockables[r])
                                    {
                                        codeCursor = i;
                                        isUnlocked[i] = true;
                                    }
                                unlockables.RemoveAt(r);
                            }
                            else
                                isAllUnlocked = true;
                            bg = Content.Load<Texture2D>("assets/blue");
                            MediaPlayer.Play(desertsuite);
                            if (codeCursor == 7)
                            {
                                MediaPlayer.Play(failcode);
                                bg = Content.Load<Texture2D>("assets/fail");
                            }
                            state = GameState.SoloPWin;
                        }
                        if (p2.score >= 5 && !isComp)
                        {
                            p1 = new Paddle(new Rectangle(0, res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, shield_blue, 0);
                            p2 = new Paddle(new Rectangle(res.X - res.pixelWidth(.0185), res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, red, 1);
                            MediaPlayer.Play(steelmill);
                            bg = Content.Load<Texture2D>("assets/red");
                            state = GameState.SoloCWin;
                        }
                        if (p1.score >= 5 && isComp)
                        {
                            p1 = new Paddle(new Rectangle(0, res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, shield_blue, 0);
                            p2 = new Paddle(new Rectangle(res.X - res.pixelWidth(.0185), res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, red, 1);
                            MediaPlayer.Play(desertsuite);
                            bg = Content.Load<Texture2D>("assets/blue");
                            state = GameState.CompP1Win;
                        }
                        if (p2.score >= 5 && isComp)
                        {
                            p1 = new Paddle(new Rectangle(0, res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, shield_blue, 0);
                            p2 = new Paddle(new Rectangle(res.X - res.pixelWidth(.0185), res.pixelHeight(.375), res.pixelWidth(.0185), res.pixelHeight(.25)), pixel, red, 1);
                            MediaPlayer.Play(steelmill);
                            bg = Content.Load<Texture2D>("assets/red");
                            state = GameState.CompP2Win;
                        }
                    }
                    break;
                case GameState.SoloPWin:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        if (unlockables.Count == 0)
                            isAllUnlocked = true;
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.SoloCWin:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.CompP1Win:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.CompP2Win:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.About:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        state = GameState.MainMenu;
                    }
                    break;
                case GameState.Code:
                    if(Keyboard.GetState().IsKeyDown(Keys.O) && Keyboard.GetState().IsKeyDown(Keys.P))//Dev cheat for gath
                    {
                        for (int i = 0; i < isUnlocked.Count; i++)
                            isUnlocked[i] = true;
                        isAllUnlocked = true;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.1557), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[0])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 0;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.1557), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[1])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 1;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.3438), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[2])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 2;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.3438), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[3])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 3;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.5318), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[4])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 4;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.5318), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[5])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 5;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.7198), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[6])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 6;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.7198), res.pixelWidth(.2924), res.pixelHeight(.1239))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && isUnlocked[7])
                    {
                        MediaPlayer.Play(message);
                        codeCursor = 7;
                        state = GameState.CodeDesc;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        codebg.reset();
                        state = GameState.MainMenu;
                    }
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(res.pixelWidth(.8684), res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(allcodes);
                        state = GameState.Send;
                    }
                    break;
                case GameState.CodeDesc:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(uvb);
                        state = GameState.Code;
                    }
                    break;
                case GameState.Send:
                    if (!sent.isTyping)
                        sent.isTyping = true;
                    sent.Update(gameTime);
                    break;
                case GameState.Credits:
                    if (new Rectangle(mouse.Position, Point.Zero).Intersects(new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125))) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        MediaPlayer.Play(trustme);
                        bg = Content.Load<Texture2D>("assets/menubg");
                        state = GameState.MainMenu;
                    }
                    break;
            }

            prevMouse = mouse;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(render);
            GraphicsDevice.Clear(Color.Black);

            switch (state)
            {
                case GameState.IntroText:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.8517)), new Color(Color.Black, .7f));

                    spriteBatch.End();
                    introText.Draw(spriteBatch, Justify.Top, Align.Left);
                    break;
                case GameState.MainMenu:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(tax, new Rectangle(res.pixelWidth(.28875), res.pixelHeight(-.2511), res.pixelWidth(.4225), res.pixelWidth(.4225)), new Color(Color.White, .4f));

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.3455), res.pixelHeight(.705), res.pixelWidth(.1404), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.3567), res.pixelHeight(.705) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5141), res.pixelHeight(.705), res.pixelWidth(.1404), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5253), res.pixelHeight(.705) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.3455), res.pixelHeight(.875), res.pixelWidth(.1404), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.3567), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5141), res.pixelHeight(.875), res.pixelWidth(.1404), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5253), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.118), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.8684), res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.8796), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    menutitle.Draw(spriteBatch, Justify.Center, Align.Center);
                    menub1.Draw(spriteBatch, Justify.Center, Align.Center);
                    menub2.Draw(spriteBatch, Justify.Center, Align.Center);
                    menub3.Draw(spriteBatch, Justify.Center, Align.Center);
                    menub4.Draw(spriteBatch, Justify.Center, Align.Center);
                    menub5.Draw(spriteBatch, Justify.Center, Align.Center);
                    quit.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.InGame:
                    spriteBatch.Begin();

                    for (int i = 0; i < res.Y / 123 + 2; i++)
                    {
                        for (int j = 0; j < res.X / 123 + 2; j++)
                        {
                            spriteBatch.Draw(grid, new Rectangle(j * 123 - 61, i * 123 - 61, 123, 123), Color.White);
                        }
                    }

                    spriteBatch.End();

                    p1.Draw(spriteBatch);
                    p2.Draw(spriteBatch);

                    b1.Draw(spriteBatch);
                    b2.Draw(spriteBatch);

                    foreach (List<Brick> brickRow in brickWall)
                    {
                        foreach (Brick i in brickRow)
                        {
                            i.Draw(spriteBatch);
                        }
                    }
                    spriteBatch.Begin();
                    spriteBatch.DrawString(alienEncounters, "Score: " + p1.score, new Vector2(p1.dims.Width + 20, 0), blue, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(alienEncounters, "Score: " + p2.score, new Vector2(res.X - p2.dims.Width - alienEncounters.MeasureString("Score: " + p2.score).X - 20, 0), red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.End();
                    break;
                case GameState.SoloPWin:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1462), res.pixelHeight(.1097), res.pixelWidth(.7076), res.pixelHeight(.7259)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    if (codeCursor != 7)
                    {
                        blueWin.Draw(spriteBatch, Justify.Center, Align.Center);
                        if (!isAllUnlocked)
                        {
                            code.Draw(spriteBatch, Justify.Center, Align.Center);
                            codesList[codeCursor].Draw(spriteBatch, Justify.Center, Align.Center);
                        }
                    }
                    else if(codeCursor == 7 && !isAllUnlocked)
                    {
                        failTitle.Draw(spriteBatch, Justify.Center, Align.Center);
                        failSubtitle.Draw(spriteBatch, Justify.Center, Align.Center);
                        failTAX.Draw(spriteBatch, Justify.Center, Align.Center);
                    }
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.SoloCWin:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1462), res.pixelHeight(.1097), res.pixelWidth(.7076), res.pixelHeight(.7259)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    redWin.Draw(spriteBatch, Justify.Center, Align.Center);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.CompP1Win:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1462), res.pixelHeight(.1097), res.pixelWidth(.7076), res.pixelHeight(.7259)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    blueWin.Draw(spriteBatch, Justify.Center, Align.Center);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.CompP2Win:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1462), res.pixelHeight(.1097), res.pixelWidth(.7076), res.pixelHeight(.7259)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    redWin.Draw(spriteBatch, Justify.Center, Align.Center);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.About:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.015), res.pixelHeight(.0533), res.pixelWidth(.9699), res.pixelHeight(.8933)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    aboutDesc.Draw(spriteBatch, Justify.Top, Align.Left);
                    aboutPan1.Draw(spriteBatch, Justify.Top, Align.Left);
                    aboutPan2.Draw(spriteBatch, Justify.Top, Align.Left);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.Code:
                    codebg.Draw(spriteBatch);

                    spriteBatch.Begin();

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.1557), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.1557) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.1557), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.1557) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.3438), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.3438) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.3438), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.3438) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.5318), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.5318) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.5318), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.5318) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.1893), res.pixelHeight(.7198), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.2005), res.pixelHeight(.7198) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5183), res.pixelHeight(.7198), res.pixelWidth(.2924), res.pixelHeight(.1239)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.5295), res.pixelHeight(.7198) + res.pixelWidth(.0112), res.pixelWidth(.27), res.pixelHeight(.1239) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    if(isAllUnlocked)
                    {
                        spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.8684), res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                        spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.8796), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);
                    }

                    spriteBatch.End();
                    for (int i = 0; i < isUnlocked.Count; i++)
                        if (isUnlocked[i])
                            codeButtons[i].Draw(spriteBatch, Justify.Center, Align.Center);
                    if (isAllUnlocked)
                        send.Draw(spriteBatch, Justify.Center, Align.Center);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.CodeDesc:
                    codebg.Draw(spriteBatch);
                    spriteBatch.Begin();

                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0512), res.pixelHeight(.07415), res.pixelWidth(.8976), res.pixelHeight(.8)), new Color(Color.Black, .7f));

                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    codeTitle[codeCursor].Draw(spriteBatch, Justify.Center, Align.Center);
                    codeDesc[codeCursor].Draw(spriteBatch, Justify.Top, Align.Left);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
                case GameState.Send:
                    codebg.Draw(spriteBatch);
                    sent.Draw(spriteBatch, Justify.Top, Align.Left);
                    break;
                case GameState.Credits:
                    spriteBatch.Begin();

                    spriteBatch.Draw(bg, new Rectangle(0, 0, res.X, res.Y), Color.White);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0687), res.pixelHeight(.1464), res.pixelWidth(.8626), res.pixelHeight(.7072)), new Color(Color.Black, .7f));
                    spriteBatch.Draw(pixel, new Rectangle(0, res.pixelHeight(.875), res.pixelWidth(.1316), res.pixelHeight(.125)), blue);
                    spriteBatch.Draw(pixel, new Rectangle(res.pixelWidth(.0112), res.pixelHeight(.875) + res.pixelWidth(.0112), res.pixelWidth(.1092), res.pixelHeight(.125) - res.pixelWidth(.0224)), shield_blue);

                    spriteBatch.End();
                    credits.Draw(spriteBatch, Justify.Top, Align.Left);
                    back.Draw(spriteBatch, Justify.Center, Align.Center);
                    break;
            }
            spriteBatch.Begin();
            spriteBatch.Draw(cursor, new Rectangle(mouse.X, mouse.Y, res.pixelHeight(.05), res.pixelHeight(.05)), Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw((Texture2D)render, new Rectangle(0, 0, res.X, res.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
