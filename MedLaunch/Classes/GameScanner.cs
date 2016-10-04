﻿using Asnitech.Launch.Common;
using MedLaunch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Windows;

namespace MedLaunch.Classes
{
    public class GameScanner
    {
        private MyDbContext db;

        // constructor
        public GameScanner()
        {
            db = new MyDbContext();

            Games = (from g in db.Game
                     select g).ToList();

            Paths = (from p in db.Paths
                     where p.pathId == 1
                     select p).ToList().SingleOrDefault();

            Systems = (from s in db.GameSystem
                       select s).ToList();

            RomSystems = new List<GameSystem>();
            DiskSystems = new List<GameSystem>();

            // populate RomSystems and DiskSystems
            foreach (GameSystem gs in Systems)
            {
                // exlude non-path systems
                if (gs.systemId == 16 || gs.systemId == 17)
                    continue;

                // populate disksystems
                if (gs.systemId == 18           // pcecd
                    || gs.systemId == 8         // pcfx
                    || gs.systemId == 9         // psx
                    || gs.systemId == 13)       // Saturn
                    DiskSystems.Add(gs);
                else
                    RomSystems.Add(gs);
            }

            RomSystemsWithPaths = new List<GameSystem>();
            DiskSystemsWithPaths = new List<GameSystem>();

            // populate RomSystemsWithPaths with only entries that only have Rom paths set (and are not non-path systems like snes_faust and pce_fast)
            foreach (var sys in RomSystems)
            {
                if (GetPath(sys.systemId) == null || GetPath(sys.systemId) == "")
                {
                    continue;
                }
                RomSystemsWithPaths.Add(sys);
            }
            /*
            for (int i = 1; RomSystems.Count >= i; i++)
            {
                if (GetPath(i) == null || GetPath(i) == "")
                    continue;

                MessageBoxResult result2 = MessageBox.Show(RomSystems[i - 1].systemName);
                RomSystemsWithPaths.Add(RomSystems[i - 1]); 
            }
            */
            // populate DiskSystemsWithPaths with only entries that only have Disk paths set (and are not non-path systems like snes_faust and pce_fast)
            foreach (var sys in DiskSystems)
            {
                if (GetPath(sys.systemId) == null || GetPath(sys.systemId) == "")
                {
                    continue;
                }
                DiskSystemsWithPaths.Add(sys);
            }

            // per system lists
            GamesGB = (from g in Games
                      where g.systemId == 1
                      select g).ToList();

            GamesGBA = (from g in Games
                       where g.systemId == 2
                       select g).ToList();

            GamesLYNX = (from g in Games
                        where g.systemId == 3
                        select g).ToList();

            GamesMD = (from g in Games
                         where g.systemId == 4
                         select g).ToList();

            GamesGG = (from g in Games
                       where g.systemId == 5
                       select g).ToList();

            GamesNGP = (from g in Games
                       where g.systemId == 6
                       select g).ToList();

            GamesPCE = (from g in Games
                       where g.systemId == 7
                       select g).ToList();

            GamesPCFX = (from g in Games
                       where g.systemId == 8
                       select g).ToList();

            GamesPSX = (from g in Games
                       where g.systemId == 9
                       select g).ToList();

            GamesSMS = (from g in Games
                       where g.systemId == 10
                       select g).ToList();

            GamesNES = (from g in Games
                       where g.systemId == 11
                       select g).ToList();

            GamesSNES = (from g in Games
                       where g.systemId == 12
                       select g).ToList();

            GamesSS = (from g in Games
                       where g.systemId == 13
                       select g).ToList();

            GamesVB = (from g in Games
                       where g.systemId == 14
                       select g).ToList();

            GamesWSWAN = (from g in Games
                       where g.systemId == 15
                       select g).ToList();

            GamesPCECD = (from g in Games
                          where g.systemId == 18
                          select g).ToList();



            RomsToUpdate = new List<Game>();
            RomsToAdd = new List<Game>();
            DisksToUpdate = new List<Game>();
            DisksToAdd = new List<Game>();
            AddedStats = 0;
            HiddenStats = 0;
            UpdatedStats = 0;
            UntouchedStats = 0;
        }

        // properties
        public List<Game> Games { get; private set; }
        public Paths Paths { get; private set; }
        public List<GameSystem> Systems { get; private set; }
        public List<GameSystem> RomSystems { get; private set; }
        public List<GameSystem> DiskSystems { get; private set; }
        public List<GameSystem> RomSystemsWithPaths { get; private set; }
        public List<GameSystem> DiskSystemsWithPaths { get; private set; }
        public List<Game> GamesGB { get; private set; }
        public List<Game> GamesGBA { get; private set; }
        public List<Game> GamesLYNX { get; private set; }
        public List<Game> GamesMD { get; private set; }
        public List<Game> GamesGG { get; private set; }
        public List<Game> GamesNGP { get; private set; }
        public List<Game> GamesPCE { get; private set; }
        public List<Game> GamesPCFX { get; private set; }
        public List<Game> GamesPSX { get; private set; }
        public List<Game> GamesSMS { get; private set; }
        public List<Game> GamesNES { get; private set; }
        public List<Game> GamesSNES { get; private set; }
        public List<Game> GamesSS { get; private set; }
        public List<Game> GamesVB { get; private set; }
        public List<Game> GamesWSWAN { get; private set; }
        public List<Game> GamesPCECD { get; private set; }

        public List<Paths> NonNullPaths { get; private set; }

        public List<Game> RomsToUpdate { get; set; }
        public List<Game> RomsToAdd { get; set; }

        public List<Game> DisksToUpdate { get; set; }
        public List<Game> DisksToAdd { get; set; }

        public int AddedStats { get; set; }
        public int HiddenStats { get; set; }
        public int UpdatedStats { get; set; }
        public int UntouchedStats { get; set; }

        // methods
        public string GetPath(int systemId)
        {
            string path = "";
            switch (systemId)
            {
                case 1:
                    path = Paths.systemGb;
                    break;
                case 2:
                    path = Paths.systemGba;
                    break;
                case 3:
                    path = Paths.systemLynx;
                    break;
                case 4:
                    path = Paths.systemMd;
                    break;
                case 5:
                    path = Paths.systemGg;
                    break;
                case 6:
                    path = Paths.systemNgp;
                    break;
                case 7:
                    path = Paths.systemPce;
                    break;
                case 8:
                    path = Paths.systemPcfx;
                    break;
                case 9:
                    path = Paths.systemPsx;
                    break;
                case 10:
                    path = Paths.systemSms;
                    break;
                case 11:
                    path = Paths.systemNes;
                    break;
                case 12:
                    path = Paths.systemSnes;
                    break;
                case 13:
                    path = Paths.systemSs;
                    break;
                case 14:
                    path = Paths.systemVb;
                    break;
                case 15:
                    path = Paths.systemWswan;
                    break;
                case 18:
                    path = Paths.systemPce;
                    break;
                default:
                    path = "";
                    break;
            }
            return path;
        }        

        // return number of files found in a directory and sub-directory (based on usingRecursion bool)
        public static int CountFiles(string path, bool usingRecursion)
        {
            int fileCount = 0;
            try
            {
                if (usingRecursion == true)
                    fileCount = Directory.EnumerateFiles(@path, "*.*", SearchOption.AllDirectories).Count();
                else
                    fileCount = Directory.EnumerateFiles(@path, "*.*").Count();
            }
            catch { }

            return fileCount;
        }

        // Start ROM scan and import process for specific system
        public void BeginRomImport(int systemId)
        {
            // get path to ROM folder
            string romFolderPath = GetPath(systemId);
            //MessageBoxResult result2 = MessageBox.Show(romFolderPath);
            // get allowed file types for this particular system
            HashSet<string> exts = GetAllowedFileExtensions(systemId);
            

            // get all files from romfolderpath and sub directories that have an allowed extension
            IEnumerable<string> romFiles = GetFiles(romFolderPath, true);
            List<string> allowedFiles = new List<string>();
            foreach (string s in exts)
            {
                foreach (string p in romFiles)
                {
                    if (p.EndsWith(s))
                    {
                        //MessageBoxResult result5 = MessageBox.Show(p);
                        allowedFiles.Add(p);
                    }
                    
                }
                /*
                List<string> fi = (from a in romFiles
                         where a.EndsWith(s)
                         select a).ToList();
                if (fi == null || fi.Count < 1) { continue; }
                allowedFiles.AddRange(fi);       
                */             
            }

            // create new final list to be populated with approved files
            List<Game> finalGames = new List<Game>();

            // now we have a list of allowed files, loop through them
            foreach (string file in allowedFiles)
            {
                // get the relative path
                string relPath = PathUtil.GetRelativePath(romFolderPath, file);
                // get just the filename
                string fileName = System.IO.Path.GetFileName(file);
                // get just the extension
                string extension = System.IO.Path.GetExtension(file).ToLower();
                // get rom name wihout extension
                string romName = fileName.Replace(extension, "");

                Game newGame = new Game();
                
                // inspect ZIP files

                //////INVESTIGATE THIS
                if (extension == ".zip")
                {
                    //MessageBoxResult result2 = MessageBox.Show(fileName);
                    bool isAllowed = false;
                    try
                    {
                        using (ZipArchive zip = ZipFile.OpenRead(file))
                        {
                            foreach (ZipArchiveEntry entry in zip.Entries)
                            {
                                if (IsFileAllowed(entry.FullName, systemId) == true)
                                {
                                    //MessageBoxResult result3 = MessageBox.Show(entry.FullName);
                                    // zip file contains at least one recognised filetype for this system
                                    isAllowed = true;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (System.IO.InvalidDataException ex)
                    {
                        // problem with the zip file
                    }
                    finally { }
                    
                    if (isAllowed == false) { continue; }
                }

                // check whether game already exists (by gameName and systemId)
                Game chkGame = (from g in Games
                                where g.systemId == systemId && g.gameName == romName
                                select g).SingleOrDefault();

                if (chkGame == null)
                {
                    // does not already exist - create new game
                    newGame.configId = 1;
                    newGame.gameName = romName;
                    newGame.gamePath = relPath;
                    newGame.hidden = false;
                    newGame.isDiskBased = false;
                    newGame.isFavorite = false;
                    newGame.systemId = systemId;

                    // add to finaGames list
                    RomsToAdd.Add(newGame);
                    // increment the added counter
                    AddedStats++;
                }
                else
                {
                    // matching game found - update it
                    if (chkGame.gamePath == relPath && chkGame.hidden == false)
                    {
                        //nothing to update - increment untouched counter
                        UntouchedStats++;
                    }
                    else
                    {
                        newGame = chkGame;
                        // update path in case it has changed location
                        newGame.gamePath = relPath;
                        // mark as not hidden
                        newGame.hidden = false;

                        // add to finalGames list
                        RomsToUpdate.Add(newGame);
                        // increment updated counter
                        UpdatedStats++;
                    }                  

                }
            }                  
        }

        public void SaveToDatabase()
        {
            using (var ndb = new MyDbContext())
            {
                db.AddRange(RomsToAdd);
                db.UpdateRange(RomsToUpdate);
                db.SaveChanges();
            }
                
               
        }

        public bool IsFileAllowed(string fileName, int systemId)
        {
            HashSet<string> exts = GetAllowedFileExtensions(systemId);
            bool isAllowed = false;
            foreach (string ext in exts)
            {
                //MessageBoxResult result3 = MessageBox.Show("Allowed extensions for systemid " + systemId + " extention: " + ext);
                if (fileName.EndsWith(ext))
                    isAllowed = true;
            }
            return isAllowed;
        }

        public HashSet<string> GetAllowedFileExtensions(int systemId)
        {
            var exts = (from g in Systems
                        where g.systemId == systemId
                        select g).SingleOrDefault();
            string archive = exts.supportedArchiveExtensions;
            string nonArchive = exts.supportedFileExtensions;

            HashSet<string> supported = new HashSet<string>();
            char c = ',';
            string[] aSplit = archive.Split(c);
            string[] nSplit = nonArchive.Split(c);
            foreach (string s in aSplit) { supported.Add(s); }
            foreach (string s in nSplit) { supported.Add(s); }

            return supported;
        }

        // get a list of files from a directory and sub-directory (based on usingRecursion bool)
        public static System.Collections.Generic.IEnumerable<string> GetFiles(string path, bool usingRecursion)
        {
            if (usingRecursion == true)
            {
                //MessageBoxResult result = MessageBox.Show(path);
                var files = Directory.GetFiles(@path, "*.*", SearchOption.AllDirectories);
                return files;
            }
            else
            {
                var files = Directory.GetFiles(@path, "*.*");
                return files;
            }
            
        }

        // mark all ROMS from a system as hidden (as long as it is not a disk based game)
        public void MarkAllRomsAsHidden(int systemId)
        {
            List<Game> games = (from g in Games
                                where g.systemId == systemId && (g.isDiskBased =! true)
                               select g).ToList();
            if (games == null)
            {
                // no games found
            }
            else
            {
                // iterate through each game
                foreach (Game game in games)
                {
                    Game newGame = game;
                    if (newGame.hidden == false)
                    {
                        newGame.hidden = true;
                        // add to GamesToUpdate to be processed later
                        RomsToUpdate.Add(newGame);
                        HiddenStats++;
                    }
                    else
                    {
                        // game is already marked as hidden
                        UntouchedStats++;
                    }
                }
            }            
        }
        
        // mark single game as hidden
        public void MarkRomAsHidden(int gameId)
        {
            Game game = (from g in Games
                                where g.gameId == gameId
                                select g).ToList().SingleOrDefault();
            if (game == null)
            {
                // no game found
            }
            else
            {                
                Game newGame = game;
                if (newGame.hidden == false)
                {
                    newGame.hidden = true;
                    // add to GamesToUpdate to be processed later
                    RomsToUpdate.Add(newGame);
                    HiddenStats++;
                } 
                else
                {
                    // game is already hidden
                    UntouchedStats++;
                }               
            }
        }





        public static List<GameSystem> GetSystems()
        {
            List<GameSystem> systems = new List<GameSystem>();
            using (var sysCon = new MyDbContext())
            {
                var sys = from s in sysCon.GameSystem
                          select s;
                foreach (GameSystem g in sys)
                {
                    systems.Add(g);
                }
                return systems;
            }
        }


        // attempt to add game to Game database
        public static int AddGame(Rom systemRom, string fullPath, string relPath, string fileName, string extension, string romName)
        {
            // check whether ROM already exists in database
            using (var romContext = new MyDbContext())
            {
                var rom = (from r in romContext.Game
                          where (r.gameName == romName) && (r.GameSystem.systemId == systemRom.gameSystem.systemId)
                          select r).SingleOrDefault();

                if (rom != null)
                {
                    // Rom already exists in database. Check whether it needs updating
                    if (rom.gamePath == relPath)
                    {
                        // path is correct in database - skip updating
                        return 0;
                    }
                    else
                    {
                        // path is incorrect in database - update record
                        Game g = rom;
                        g.gamePath = relPath;

                        UpdateRom(g);
                        return 2;
                    }
                }
                else
                {
                    // Rom does not exist. Add to database.
                    Game g = new Game();
                    g.gameName = romName;
                    g.gamePath = relPath;
                    g.systemId = systemRom.gameSystem.systemId;
                    //g.GameSystem.systemId = systemRom.gameSystem.systemId;
                    g.isFavorite = false;
                    g.configId = 1;
                    g.hidden = false;

                    InsertRom(g);
                    return 1;
                }
            }
            

        }

        private static void InsertRom(Game rom)
        {
            using (var iR = new MyDbContext())
            {
                iR.Game.Add(rom);
                iR.SaveChanges();
                iR.Dispose();
            }
        }
        private static void UpdateRom(Game rom)
        {
            using (var uR = new MyDbContext())
            {
                uR.Game.Update(rom);
                uR.SaveChanges();
                uR.Dispose();
            }
        }

        // get favorite status
        public static int GetFavoriteStatus(int Id)
        {
            using (var romContext = new MyDbContext())
            {
                var rom = (from r in romContext.Game
                           where r.gameId == Id
                           select r).SingleOrDefault();

                if (rom != null)
                {
                    if (rom.isFavorite == true)
                    {
                        romContext.Dispose();
                        Debug.WriteLine("FAVOTIE!");
                        return 1;
                    }
                    else
                    {
                        romContext.Dispose();
                        return 0;
                    }
                }
                else
                { 
                romContext.Dispose();
                return 0;
                }

            }
        }

        // update favorites toggle
        public static void FavoriteToggle(int Id)
        {
            using (var romaContext = new MyDbContext())
            {
                Game rom = (from r in romaContext.Game
                            where r.gameId == Id
                            select r).SingleOrDefault();

                if (rom != null)
                {
                    if (GetFavoriteStatus(Id) == 1)
                    {
                        // Rom is marked as a favorite - make isFavorite as false
                        rom.isFavorite = false;
                    }
                    else
                    {
                        // rom is not marked as favorite - make isFavorite true
                        rom.isFavorite = true;
                    }
                }

                // update ROM
                UpdateRom(rom);

                romaContext.Dispose();
            }
        }
    }
}