using System;
using System.Collections.Generic;
using System.Linq;
using Bots.DungeonBuddy.Enums;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.Combat.CombatRoutine; // for WoWClass enum

namespace Bots.DungeonBuddy
{
    /// <summary>
    /// Gère l'interface avec le Dungeon Finder (LFG).
    /// Compatible WotLK 3.3 - Dungeon Finder ajouté en patch 3.3.
    /// </summary>
    public static class LfgManager
    {
        // ═══════════════════════════════════════════════════════════
        // LFG STATE
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// État actuel du LFG.
        /// Utilise GetLFGMode() — API canonique confirmée WotLK 3.3.
        /// NOTE: En WotLK 3.3, GetLFGMode() ne prend AUCUN argument
        ///       (le paramètre category a été ajouté en MoP 5.x).
        /// Retourne: "queued", "proposal", "lfgparty", "abandonedInDungeon",
        ///           "suspended", "rolecheck", ou nil
        /// C'est la même méthode utilisée par HB 4.3.4 DungeonBuddy.
        /// </summary>
        public static LfgState CurrentState
        {
            get
            {
                string mode = Lua.GetReturnVal<string>(
                    "return GetLFGMode() or 'none'", 0);
                
                return mode?.ToLowerInvariant() switch
                {
                    "queued"              => LfgState.InQueue,
                    "proposal"            => LfgState.Proposal,
                    "lfgparty"            => LfgState.InDungeon,
                    "abandonedindungeon"  => LfgState.AbandonedInDungeon,
                    "suspended"           => LfgState.Suspended,
                    "rolecheck"           => LfgState.RoleCheck,
                    _                     => LfgState.NotInLfg
                };
            }
        }

        /// <summary>
        /// Temps d'attente estimé (en secondes)
        /// </summary>
        public static int EstimatedWaitTime
        {
            get
            {
                // WotLK 3.3.5a: category 1 = LFD (pas de constante LE_LFG_CATEGORY_LFD)
                // NOTE: La 16e valeur retour (waitTime estimé) n'est pas documentée sur Wowpedia 
                // pour WotLK. La position exacte est déduite de HB 3.3.5a. À valider en jeu.
                return Lua.GetReturnVal<int>(
                    "local _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,w=GetLFGQueueStats(1); return w or 0", 0);
            }
        }

        // ═══════════════════════════════════════════════════════════
        // QUEUE ACTIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Queue pour un donjon aléatoire
        /// </summary>
        public static void QueueForRandomDungeon()
        {
            Logging.Write("[DungeonBuddy] Queuing for Random Dungeon via UI (Instancebuddy style)...");
            
            // Just like Instancebuddy, we rely on the user having selected their roles and the random dungeon manually.
            Lua.DoString(@"
                if not LFDQueueFrame then LoadAddOn('Blizzard_LFGUI') end
                LFDQueueFrame_Join();
            ");
        }

        /// <summary>
        /// Queue pour un héroïque aléatoire
        /// </summary>
        public static void QueueForRandomHeroic()
        {
            Logging.Write("[DungeonBuddy] Queuing for Random Heroic via UI (Instancebuddy style)...");
            
            Lua.DoString(@"
                if not LFDQueueFrame then LoadAddOn('Blizzard_LFGUI') end
                LFDQueueFrame_Join();
            ");
        }

        /// <summary>
        /// Queue pour un donjon spécifique
        /// </summary>
        public static void QueueForSpecificDungeon(uint dungeonId)
        {
            Logging.Write($"[DungeonBuddy] Queuing for dungeon ID {dungeonId}...");
            
            Lua.DoString($@"
                ClearAllLFGDungeons();
                SetLFGDungeon({dungeonId}, true);
                JoinLFG();
            ");
        }

        /// <summary>
        /// Accepter la proposition de donjon.
        /// Parité HB: appel direct Lua AcceptProposal().
        /// </summary>
        public static void AcceptProposal()
        {
            Logging.Write("[DungeonBuddy] Accepting dungeon proposal...");
            Lua.DoString("AcceptProposal()");
        }

        /// <summary>
        /// Refuser la proposition
        /// </summary>
        public static void DeclineProposal()
        {
            Logging.Write("[DungeonBuddy] Declining dungeon proposal...");
            Lua.DoString("RejectProposal()");
        }

        /// <summary>
        /// Quitter la queue
        /// </summary>
        public static void LeaveQueue()
        {
            Logging.Write("[DungeonBuddy] Leaving LFG queue...");
            Lua.DoString("LeaveLFG()");
        }

        // ═══════════════════════════════════════════════════════════
        // TELEPORT
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Téléporte dans le donjon
        /// </summary>
        public static void TeleportIn()
        {
            Logging.Write("[DungeonBuddy] Teleporting into dungeon...");
            // LFGTeleport(toSafety) — false = teleport INTO dungeon
            // Wowpedia: toSafety=false to teleport to the dungeon
            Lua.DoString("LFGTeleport(false)");
        }

        /// <summary>
        /// Téléporte hors du donjon
        /// </summary>
        public static void TeleportOut()
        {
            Logging.Write("[DungeonBuddy] Teleporting out of dungeon...");
            Lua.DoString("LFGTeleport(true)");
        }

        // ═══════════════════════════════════════════════════════════
        // ROLE SELECTION
        // ═══════════════════════════════════════════════════════════

        private static PartyRole _currentRole = PartyRole.Dps;

        /// <summary>
        /// Définit le rôle pour le LFG
        /// </summary>
        public static void SetRole(PartyRole role)
        {
            // Removed to prevent "You did not select any valid slots" error.
            // The user manually selects their role in the UI, just like in the original Instancebuddy.
            _currentRole = role;
        }

        /// <summary>
        /// Obtient les rôles disponibles pour la classe actuelle
        /// </summary>
        public static PartyRole GetAvailableRoles()
        {
            var result = PartyRole.None;
            
            // Toutes les classes peuvent DPS
            result |= PartyRole.Dps;
            
            // Tanks: Warrior, Paladin, Death Knight, Druid (Bear)
            var tankClasses = new WoWClass[] { WoWClass.Warrior, WoWClass.Paladin, WoWClass.DeathKnight, WoWClass.Druid };
            if (tankClasses.Contains(StyxWoW.Me.Class))
                result |= PartyRole.Tank;
            
            // Healers: Priest, Paladin, Shaman, Druid
            var healerClasses = new WoWClass[] { WoWClass.Priest, WoWClass.Paladin, WoWClass.Shaman, WoWClass.Druid };
            if (healerClasses.Contains(StyxWoW.Me.Class))
                result |= PartyRole.Healer;
            
            return result;
        }

        // ═══════════════════════════════════════════════════════════
        // DUNGEON INFO
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Obtient le MapID du donjon actuel
        /// </summary>
        public static uint CurrentMapId => (uint)StyxWoW.Me.MapId;

        /// <summary>
        /// ID du donjon LFG actuel (depuis GetPartyLFGID()).
        /// Retourne 0 si pas dans un groupe LFG.
        /// HB 4.3.4: DungeonManager utilise cette valeur pour choisir le script de donjon.
        /// </summary>
        // Use pcall to avoid Lua runtime error (status=2) when not in an LFG group.
        // GetPartyLFGID() throws in WotLK 3.3.5a when no LFG instance is active.
        public static uint CurrentLfgDungeonId => Lua.GetReturnVal<uint>("local ok,v = pcall(GetPartyLFGID); return (ok and v) or 0", 0);

        /// <summary>
        /// Obtient la difficulté du donjon actuel (1=Normal, 2=Heroic)
        /// </summary>
        public static int CurrentDifficulty
        {
            get
            {
                return Lua.GetReturnVal<int>("return GetInstanceDifficulty()", 0);
            }
        }

        /// <summary>
        /// Liste des donjons disponibles pour le niveau actuel.
        /// NOTE: GetDungeonInfo(i) existe en WotLK mais n'est PAS GetLFGDungeonInfo().
        /// GetNumDungeons() et GetDungeonInfo() sont des API de LFDFrame.
        /// </summary>
        public static List<(uint Id, string Name, bool IsHeroic)> GetAvailableDungeons()
        {
            var result = new List<(uint, string, bool)>();
            
            var luaResult = Lua.GetReturnVal<string>(@"
                local dungeons = '';
                for i = 1, GetNumDungeons() do
                    local name, typeID, subtypeID, minLevel, maxLevel, _, minRecLevel, maxRecLevel,
                          _, _, _, difficulty = GetDungeonInfo(i);
                    if UnitLevel('player') >= minLevel and UnitLevel('player') <= maxLevel then
                        dungeons = dungeons .. i .. '|' .. name .. '|' .. difficulty .. ';';
                    end
                end
                return dungeons;
            ", 0);

            if (string.IsNullOrEmpty(luaResult))
                return result;

            foreach (var entry in luaResult.Split(';').Where(s => !string.IsNullOrEmpty(s)))
            {
                var parts = entry.Split('|');
                if (parts.Length >= 3)
                {
                    uint id = uint.Parse(parts[0]);
                    string name = parts[1];
                    bool isHeroic = parts[2] == "2";
                    result.Add((id, name, isHeroic));
                }
            }

            return result;
        }

        // ═══════════════════════════════════════════════════════════
        // LFG EVENTS (via Lua.Events — confirmé dans CopilotBuddy)
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Flag: un proposal est en attente (HB bool_6)
        /// </summary>
        public static bool ProposalPending { get; set; }

        /// <summary>
        /// Flag: proposal failed (HB bool_4)
        /// </summary>
        public static bool ProposalFailed { get; set; }

        /// <summary>
        /// Flag: proposal succeeded (HB bool_5)
        /// </summary>
        public static bool ProposalSucceeded { get; set; }

        /// <summary>
        /// Flag: resurrect request (HB bool_3)
        /// </summary>
        public static bool ResurrectRequestPending { get; set; }

        /// <summary>
        /// Flag: role check popup shown (HB bool_8)
        /// </summary>
        public static bool RoleCheckPending { get; set; }

        /// <summary>
        /// Flag: party invite request (HB bool_9)
        /// </summary>
        public static bool PartyInvitePending { get; set; }

        /// <summary>
        /// Exit delay timer (HB waitTimer_7) — set on DungeonCompleted to trigger leave
        /// </summary>
        public static readonly WaitTimer ExitDelayTimer = new WaitTimer(TimeSpan.FromSeconds(60));

        /// <summary>
        /// Post-proposal delay timer (HB waitTimer_9) — reset when proposal succeeds
        /// </summary>
        internal static readonly WaitTimer PostProposalTimer = new WaitTimer(TimeSpan.FromSeconds(8.0));

        /// <summary>
        /// Completion reason enum (HB completeReason_0)
        /// </summary>
        public static CompleteReason DungeonCompletedReason { get; set; } = CompleteReason.None;

        /// <summary>
        /// Flag: LFG_COMPLETION_REWARD received (HB bool_7)
        /// </summary>
        public static bool BootProposalActive { get; set; }

        /// <summary>
        /// Attache les événements LFG via Lua.Events.AttachEvent
        /// Appelé dans DungeonBuddy.Start()
        /// </summary>
        public static void AttachLfgEvents()
        {
            ProposalPending = false;
            ProposalFailed = false;
            ProposalSucceeded = false;
            ResurrectRequestPending = false;
            RoleCheckPending = false;
            PartyInvitePending = false;
            DungeonCompletedReason = CompleteReason.None;
            BootProposalActive = false;

            Lua.Events.AttachEvent("LFG_PROPOSAL_SHOW", OnProposalShow);
            Lua.Events.AttachEvent("LFG_PROPOSAL_FAILED", OnProposalFailed);
            Lua.Events.AttachEvent("LFG_PROPOSAL_SUCCEEDED", OnProposalSucceeded);
            Lua.Events.AttachEvent("LFG_OFFER_CONTINUE", OnOfferContinue);
            Lua.Events.AttachEvent("LFG_ROLE_CHECK_SHOW", OnRoleCheck);
            Lua.Events.AttachEvent("LFG_COMPLETION_REWARD", OnCompletionReward);
            Lua.Events.AttachEvent("RESURRECT_REQUEST", OnResurrectRequest);
            Lua.Events.AttachEvent("PARTY_INVITE_REQUEST", OnPartyInviteRequest);
            Lua.Events.AttachEvent("LFG_INVALID_ERROR_MESSAGE", OnLfgInvalidErrorMessage);
        }

        /// <summary>
        /// Détache les événements LFG.
        /// Appelé dans DungeonBuddy.Stop()
        /// </summary>
        public static void DetachLfgEvents()
        {
            Lua.Events.DetachEvent("LFG_PROPOSAL_SHOW", OnProposalShow);
            Lua.Events.DetachEvent("LFG_PROPOSAL_FAILED", OnProposalFailed);
            Lua.Events.DetachEvent("LFG_PROPOSAL_SUCCEEDED", OnProposalSucceeded);
            Lua.Events.DetachEvent("LFG_OFFER_CONTINUE", OnOfferContinue);
            Lua.Events.DetachEvent("LFG_ROLE_CHECK_SHOW", OnRoleCheck);
            Lua.Events.DetachEvent("LFG_COMPLETION_REWARD", OnCompletionReward);
            Lua.Events.DetachEvent("RESURRECT_REQUEST", OnResurrectRequest);
            Lua.Events.DetachEvent("PARTY_INVITE_REQUEST", OnPartyInviteRequest);
            Lua.Events.DetachEvent("LFG_INVALID_ERROR_MESSAGE", OnLfgInvalidErrorMessage);
        }

        private static void OnProposalShow(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] LFG proposal received!");
            ProposalPending = true;
        }

        private static void OnProposalFailed(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] LFG proposal failed/declined");
            ProposalFailed = true;
        }

        private static void OnProposalSucceeded(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] LFG proposal accepted by all! Teleporting...");
            PostProposalTimer.Reset();
            ProposalSucceeded = true;
        }

        private static void OnOfferContinue(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] LFG offer to continue (requeue)");
            BootProposalActive = true;
        }

        private static void OnRoleCheck(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] Role check requested, accepting...");
            RoleCheckPending = true;
        }

        private static void OnCompletionReward(object sender, LuaEventArgs args)
        {
            int delay = DungeonBuddySettings.Instance.PartyMode == PartyMode.Off ? 30 : 10;
            SetDungeonCompleted(CompleteReason.Completed, delay);
            Logging.Write("[DungeonBuddy] Dungeon completed! Leaving in {0} seconds.", delay);
        }

        private static void OnResurrectRequest(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] Resurrection request received");
            ResurrectRequestPending = true;
        }

        private static void OnPartyInviteRequest(object sender, LuaEventArgs args)
        {
            Logging.Write("[DungeonBuddy] Party invite request received");
            PartyInvitePending = true;
        }

        public static void ResetProposalFlags()
        {
            ProposalPending = false;
            ProposalFailed = false;
            ProposalSucceeded = false;
        }

        private static void OnLfgInvalidErrorMessage(object sender, LuaEventArgs args)
        {
            try
            {
                var error = (LfgInvalidError)args.Args[0];
                string text = args.Args[1] as string;
                string text2 = args.Args[2] as string;
                Logging.Write("[DungeonBuddy] Lfg queue failed. {0}: {1}, {2}", error, text ?? "", text2 ?? "");
            }
            catch
            {
                Logging.Write("[DungeonBuddy] Lfg queue failed (error parsing args)");
            }
        }

        /// <summary>
        /// HB smethod_23: set DungeonCompleted + reset ExitDelayTimer
        /// </summary>
        public static void SetDungeonCompleted(CompleteReason reason, int delaySeconds = 60)
        {
            ExitDelayTimer.WaitTime = TimeSpan.FromSeconds(delaySeconds);
            if (reason != CompleteReason.None)
                ExitDelayTimer.Reset();
            DungeonCompletedReason = reason;
        }
    }
}