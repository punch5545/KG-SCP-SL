namespace KG_SCP_SL
{
    using AdminToys;
    using CommandSystem;
    using CustomPlayerEffects;
    using Footprinting;
    using Interactables.Interobjects;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Radio;
    using InventorySystem.Items.Usables;
    using KG_SCP_SL.Factory;
    using LiteNetLib;
    using MapGeneration.Distributors;
    using PlayerRoles;
    using PlayerRoles.RoleAssign;
    using PlayerRoles.Voice;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Timers;
    using UnityEngine;

    public class Main
    {

        private List<Player> Dclass = new List<Player>();
        private List<Player> Scientist = new List<Player>();
        private List<Player> MTF = new List<Player>();
        private List<Player> SCP = new List<Player>();
        private List<Player> CI = new List<Player>();

        private bool isEscaped = false;

        Timer TPS = new Timer();

        string timeStr1 = "00";
        string timeStr2 = "00";
        uint time = 0;

        public static Main Singleton { get; private set; }


        private int AddList(Player player, Team team)
        {
            int teamCount = 0;
            switch (team)
            {
                case Team.ClassD:
                    Dclass.Add(player);
                    teamCount = Dclass.Count;
                    break;
                case Team.Scientists:
                    Scientist.Add(player);
                    teamCount = Scientist.Count;
                    break;
                case Team.SCPs:
                    SCP.Add(player);
                    teamCount = SCP.Count;
                    break;
                case Team.FoundationForces:
                    MTF.Add(player);
                    teamCount = MTF.Count;
                    break;
                case Team.ChaosInsurgency:
                    CI.Add(player);
                    teamCount = CI.Count;
                    break;
                default:
                    break;
            }
            return teamCount;
        }

        private int RemoveList(Player player, Team team)
        {
            int teamCount = 0;
            switch (team)
            {
                case Team.ClassD:
                    Dclass.Remove(player);
                    teamCount = Dclass.Count;
                    break;
                case Team.Scientists:
                    Scientist.Remove(player);
                    teamCount = Scientist.Count;
                    break;
                case Team.SCPs:
                    SCP.Remove(player);
                    teamCount = SCP.Count;
                    break;
                case Team.FoundationForces:
                    MTF.Remove(player);
                    teamCount = MTF.Count;
                    break;
                case Team.ChaosInsurgency:
                    CI.Remove(player);
                    teamCount = CI.Count;
                    break;
                default:
                    break;
            }
            return teamCount;
        }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("KG Plugin", "1.0.0", "For KG Server", "KG")]
        void LoadPlugin()
        {
            Singleton = this;
            // Log.Info("Loaded plugin, register events...");
            EventManager.RegisterEvents(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            FactoryManager.RegisterPlayerFactory(this, new KGPlayerFactory());
            // Log.Info("Registered player factory!");

            var handler = PluginHandler.Get(this);
            
            TPS.Interval = 1000;
            TPS.Elapsed += TPS_Elapsed;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        void OnPlayerJoin(KGPlayer player)
        {
            // Log.Info($"Player &6{player.UserId}&r joined this server with &1{player.Test}&4");

            //foreach (var plr in Player.GetPlayers())
            //{
            //    // Log.Info($"Player online &6{plr.Nickname}&r, role &6{plr.Role}&r");
            //}
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        void OnWaitingForPlayers()
        {

        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(KGPlayer player)
        {
            // Log.Info($"Player &6{player.UserId}&r left this server with of &1{player.Test}&4");
        }

        [PluginEvent(ServerEventType.PlayerDying)]
        bool OnPlayerDying(KGPlayer player, KGPlayer attacker, DamageHandlerBase damageHandler)
        {
            var condition = false;
            if (condition is true)
            {
                // The event is canceled and the player does not die
                return false;
            }
            else
            {
                 //Log.Info(attacker == null
                 //   ? $"Player &6{player.Nickname}&r (&6{player.UserId}&r) is dying, cause {damageHandler}"
                 //   : $"Player &6{player.Nickname}&r (&6{player.UserId}&r) is dying by &6{attacker.Nickname}&r (&6{attacker.UserId}&r), cause {damageHandler}");
                // The event runs normally
                return true;
            }
        }
        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        void OnLczDecontaminationStarts()
        {
            // Log.Info("Started LCZ decontamination.");
        }

        [PluginEvent(ServerEventType.LczDecontaminationAnnouncement)]
        void OnAnnounceLczDecontamination(int id)
        {
            // Log.Info($"LCZ Annoucement &6{id}&r.");
        }

        [PluginEvent(ServerEventType.MapGenerated)]
        void OnMapGenerated()
        {
            // Log.Info("Map generated.");
        }

        [PluginEvent(ServerEventType.GrenadeExploded)]
        void OnGrenadeExploded(Footprint owner, Vector3 position, ItemPickupBase item)
        {
            // Log.Info($"Grenade &6{item.NetworkInfo.ItemId}&r thrown by &6{item.PreviousOwner.Nickname}&r exploded at &6{item.NetworkInfo.RelativePosition.ToString()}&r");
        }

        [PluginEvent(ServerEventType.ItemSpawned)]
        void OnItemSpawned(ItemType item, Vector3 position)
        {
            // Log.Info($"Item &6{item}&r spawned on map");
        }

        short genCount = 0;

        [PluginEvent(ServerEventType.GeneratorActivated)]
        void OnGeneratorActivated(Scp079Generator gen)
        {
            genCount++;
            foreach (var plr in Player.GetPlayers())
            {
                plr.SendBroadcast($"<size=24>발전기 가동\n{genCount} / 3</size>", 2000);
            }
        }

        [PluginEvent(ServerEventType.PlaceBlood)]
        void OnPlaceBlood(KGPlayer player, Vector3 position)
        {
            // Log.Info($"Player &6{player.Nickname}&r blood placed on map position &6{position}&r");
        }

        [PluginEvent(ServerEventType.PlaceBulletHole)]
        void OnPlaceBulletHole(Vector3 position)
        {
            // Log.Info($"Bullet hole has been placed on map. Position &6{position}&r.");
        }

        [PluginEvent(ServerEventType.PlayerActivateGenerator)]
        void OnPlayerActivateGenerator(KGPlayer plr, Scp079Generator gen)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) activated generator with remaining time &6{gen.RemainingTime}&r");
        }

        [PluginEvent(ServerEventType.PlayerAimWeapon)]
        void OnPlayerAimsWeapon(KGPlayer plr, Firearm gun, bool isAiming)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) is {(isAiming ? "aiming" : "not aiming")} gun &6{gun.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerBanned)]
        void OnPlayerBanned(KGPlayer plr, ICommandSender issuer, string reason, long duration)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) got banned by &6{issuer.LogName}&r with reason &6{reason}&r for duration &6{duration}&r seconds");
        }

        [PluginEvent(ServerEventType.PlayerCancelUsingItem)]
        void OnPlayerCancelsUsingItem(KGPlayer plr, UsableItem item)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) cancelled using item &6{item.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerChangeItem)]
        void OnPlayerChangesItem(KGPlayer plr, ushort oldItem, ushort newItem)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) change current item &6{oldItem}&r to &6{newItem}&r");
        }

        [PluginEvent(ServerEventType.PlayerChangeRadioRange)]
        void OnPlayerChangesRadioRange(KGPlayer plr, RadioItem item, byte range)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) changed radio range to &6{range}&r");
        }

        [PluginEvent(ServerEventType.PlayerRadioToggle)]
        void OnPlayerRadioToggle(KGPlayer plr, RadioItem item, bool newState)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) toggled the radio state to &6{newState}&r");
        }

        [PluginEvent(ServerEventType.PlayerUsingRadio)]
        void OnPlayerUsingRadio(KGPlayer player, RadioItem radio, float drain)
        {
            // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) is using a radio and its draining &6{drain}&r of the battery");
        }

        [PluginEvent(ServerEventType.CassieAnnouncesScpTermination)]
        void OnCassieAnnouncScpTermination(KGPlayer scp, DamageHandlerBase damage, string announcement)
        {
            // Log.Info($"Cassie announce a SCP termination of player &6{scp.Nickname}&r (&6{scp.UserId}&r), CASSIE announcement is &6{announcement}&r");
        }

        [PluginEvent(ServerEventType.PlayerGetGroup)]
        void OnPlayerChangeGroup(string userID, UserGroup group)
        {
            // Log.Info($"User group of {userID} is &6{((group == null || group.BadgeText == null) ? "(null)" : group.BadgeText)}&r");
        }

        [PluginEvent(ServerEventType.PlayerUsingIntercom)]
        void OnPlayerUsingIntercom(KGPlayer player, IntercomState state)
        {
            // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) is using Intercom");
        }

        [PluginEvent(ServerEventType.PlayerChangeSpectator)]
        void OnPlayerChangesSpectatedPlayer(KGPlayer plr, KGPlayer oldTarget, KGPlayer newTarget)
        {
            if (oldTarget == null && newTarget != null)
            {
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) is now spectating &6{newTarget.Nickname}&r (&6{newTarget.UserId}&r)");

            }
            else if (oldTarget != null && newTarget != null)
            {
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) changed spectated player &6{oldTarget.Nickname}&r (&6{oldTarget.UserId}&r) &6{newTarget.Nickname}&r (&6{newTarget.UserId}&r)");
            }
        }

        [PluginEvent(ServerEventType.PlayerCloseGenerator)]
        void OnPlayerClosesGenerator(KGPlayer plr, Scp079Generator gen)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) closed generator");
        }

        [PluginEvent(ServerEventType.PlayerDamagedShootingTarget)]
        void OnPlayerDamagedShootingTarget(KGPlayer plr, ShootingTarget target, DamageHandlerBase dmgHandler, float amount)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) hit shooting target with damage amount &6{amount}&r");
        }

        [PluginEvent(ServerEventType.PlayerDamagedWindow)]
        void OnPlayerDamagedWindow(KGPlayer plr, BreakableWindow window, DamageHandlerBase dmgHandler, float amount)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) damaged window with damage amount &6{amount}&r");
        }

        [PluginEvent(ServerEventType.PlayerDeactivatedGenerator)]
        void OnPlayerDeactivatedGenerator(KGPlayer plr, Scp079Generator gen)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) deactivated a generator.");
        }

        [PluginEvent(ServerEventType.PlayerDropAmmo)]
        void OnPlayerDroppedAmmo(KGPlayer plr, ItemType ammoType, int amount)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) dropped &6{amount}&r ammo of type &6{ammoType}&r.");
        }

        [PluginEvent(ServerEventType.PlayerDropItem)]
        void OnPlayerDroppedItem(KGPlayer plr, ItemBase item)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) dropped item &6{item.ItemTypeId}&r.");
        }

        [PluginEvent(ServerEventType.PlayerDryfireWeapon)]
        void OnPlayerDryfireWeapon(KGPlayer plr, Firearm item)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) dryfired weapon &6{item.ItemTypeId}&r.");
        }

        [PluginEvent(ServerEventType.PlayerEscape)]
        void OnPlayerEscaped(KGPlayer plr, RoleTypeId role)
        {

        }

        [PluginEvent(ServerEventType.PlayerHandcuff)]
        void OnPlayerHandcuffed(KGPlayer plr, KGPlayer target)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) handcuffed &6{target.Nickname}&r (&6{target.UserId}&r).");
        }

        [PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
        void OnPlayerUncuffed(KGPlayer plr, KGPlayer target)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) uncuffed &6{target.Nickname}&r (&6{target.UserId}&r).");
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        void OnPlayerDamage(KGPlayer player, KGPlayer attacker, DamageHandlerBase damageHandler)
        {
            //if (attacker == null)
                // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) got damaged, cause {damageHandler}.");
            //else
                // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) received damage from &6{attacker.Nickname}&r (&6{attacker.UserId}&r), cause {damageHandler}.");
        }

        [PluginEvent(ServerEventType.PlayerKicked)]
        void OnPlayerKicked(KGPlayer plr, ICommandSender issuer, string reason)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) kicked from server by &6{issuer.LogName}&r with reason &6{reason}&r.");
        }

        [PluginEvent(ServerEventType.PlayerOpenGenerator)]
        void OnPlayerOpenedGenerator(KGPlayer plr, Scp079Generator gen)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) opened generator.");
        }


        [PluginEvent(ServerEventType.PlayerPickupAmmo)]
        void OnPlayerPickupAmmo(KGPlayer plr, ItemPickupBase pickup)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) pickup ammo {pickup.Info.ItemId}.");
        }

        [PluginEvent(ServerEventType.PlayerPickupArmor)]
        void OnPlayerPickupArmor(KGPlayer plr, ItemPickupBase pickup)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) pickup armor {pickup.Info.ItemId}.");
        }

        [PluginEvent(ServerEventType.PlayerPickupScp330)]
        void OnPlayerPickupScp330(KGPlayer plr, ItemPickupBase pickup)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) pickup scp330 {pickup.Info.ItemId}.");
        }

        [PluginEvent(ServerEventType.PlayerPreauth)]
        void OnPreauth(string userid, string ipAddress, long expiration, CentralAuthPreauthFlags flags, string country, byte[] signature, ConnectionRequest req, Int32 index)
        {
            // Log.Info($"Player &6{userid}&r (&6{ipAddress}&r) preauthenticated from country &6{country}&r with central flags &6{flags}&r");
        }

        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        void OnReceiveEffect(KGPlayer plr, StatusEffectBase effect, byte intensity, float duration)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) received effect &6{effect}&r with an intensity of &6{intensity}&r.");
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        void OnReloadWeapon(KGPlayer plr, Firearm gun)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) reloaded weapon &6{gun.ItemTypeId}&r.");
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void OnChangeRole(KGPlayer plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {

            int count = RemoveList(plr, oldRole.Team);
            AddList(plr, newRole.GetTeam());

            if((
                (oldRole.RoleTypeId == RoleTypeId.ClassD && newRole.GetTeam() == Team.ChaosInsurgency) ||
                (oldRole.RoleTypeId == RoleTypeId.Scientist && newRole.GetTeam() == Team.FoundationForces)) && 
                reason == RoleChangeReason.Escaped && isEscaped == false
            )
            {
                isEscaped = true;

                foreach(var p in KGPlayer.GetPlayers())
                {
                    p.SendBroadcast("인원 순수 탈출\n핵 발동 계획 변경", 2000);
                }
            }

            if (count == 1)// && reason == RoleChangeReason.Died || reason == RoleChangeReason.Escaped || reason == RoleChangeReason.RemoteAdmin)
            {
                switch (oldRole.Team)
                {
                    case Team.SCPs:
                        foreach (var pl in SCP)
                        {
                            Log.Info($"{pl.Nickname}이 마지막 SCP입니다.");
                            pl.SendBroadcast("<size=24>마지막 <color=red>SCP</color></size>", 2000);
                        }
                        break;
                    case Team.ChaosInsurgency:
                        foreach (var pl in CI)
                        {
                            Log.Info($"{pl.Nickname}이 마지막 반란입니다.");
                            pl.SendBroadcast("<size=24>마지막 <color=green>혼돈의 반란</color></size>", 2000);
                        }
                        break;

                    case Team.Scientists:
                        foreach (var pl in Scientist)
                        {
                            Log.Info($"{pl.Nickname}이 마지막 과학자입니다.");
                            pl.SendBroadcast("<size=24>마지막 <color=yellow>과학자</color></size>", 2000);
                        }
                        break;
                    case Team.ClassD:
                        foreach (var pl in Dclass)
                        {
                            Log.Info($"{pl.Nickname}이 마지막 죄수입니다.");
                            pl.SendBroadcast("<size=24>마지막 <color=orange>죄수</color></size>", 2000);
                        }
                        break;
                    case Team.FoundationForces:
                        foreach (var pl in MTF)
                        {
                            Log.Info($"{pl.Nickname}이 마지막 MTF입니다.");
                            pl.SendBroadcast("<size=24>마지막 <color=blue>MTF</color></size>", 2000);
                        }
                        break;
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerSearchPickup)]
        void OnSearchPickup(KGPlayer plr, ItemPickupBase pickup)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started searching pickup &6{pickup.NetworkInfo.ItemId}&r");
        }

        [PluginEvent(ServerEventType.PlayerSearchedPickup)]
        void OnSearchedPickup(KGPlayer plr, ItemPickupBase pickup)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) searched pickup &6{pickup.NetworkInfo.ItemId}&r");
        }


        [PluginEvent(ServerEventType.PlayerShotWeapon)]
        void OnShotWeapon(KGPlayer plr, Firearm gun)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) shot &6{gun.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        void OnSpawn(KGPlayer plr, RoleTypeId role)
        {

        }

        [PluginEvent(ServerEventType.PlayerThrowItem)]
        void OnThrowItem(KGPlayer plr, ItemBase item, Rigidbody rb)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) thrown item &6{item.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerToggleFlashlight)]
        void OnToggleFlashlight(KGPlayer plr, ItemBase item, bool isToggled)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) toggled {(isToggled ? "on" : "off")} flashlight on &6{item.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerUnloadWeapon)]
        void OnUnloadWeapon(KGPlayer plr, Firearm gun)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) unloads &6{gun.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        void OnUnlockGenerator(KGPlayer plr, Scp079Generator generator)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) unlocked generator");
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        void OnPlayerUsedItem(KGPlayer plr, ItemBase item)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) used item &6{item.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerUseHotkey)]
        void OnPlaeyrUsedHotkey(KGPlayer plr, ActionName action)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) used hotkey &6{action}&r");
        }

        [PluginEvent(ServerEventType.PlayerUseItem)]
        void OnPlayerStartedUsingItem(KGPlayer plr, UsableItem item)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item &6{item.ItemTypeId}&r");
        }

        [PluginEvent(ServerEventType.PlayerCheaterReport)]
        void OnCheaterReport(KGPlayer plr, KGPlayer target, string reason)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) reported player &6{target.Nickname}&r (&6{target.UserId}&r) for cheating with reason &6{reason}&r");
        }

        [PluginEvent(ServerEventType.PlayerReport)]
        void OnReport(KGPlayer plr, KGPlayer target, string reason)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) reported player &6{target.Nickname}&r (&6{target.UserId}&r) for breaking server rules with reason &6{reason}&r");
        }

        [PluginEvent(ServerEventType.PlayerInteractShootingTarget)]
        void OnInteractWithShootingTarget(KGPlayer plr, ShootingTarget target)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) interacted with shooting target in the position {target.transform.position}");
        }

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        void OnInteractWithLocker(KGPlayer plr, Locker locker, LockerChamber chamber, bool canAccess)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) {(canAccess ? "interacted" : "failed to interact")} with locker and chamber is in the position {chamber.transform.position}.");
        }

        [PluginEvent(ServerEventType.PlayerInteractElevator)]
        void OnInteractWithElevator(KGPlayer plr, ElevatorChamber elevator)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) interacted with elevator in position &6{elevator.transform.position}&r with the destination in &6{elevator.CurrentDestination.transform.position}&r");
        }

        [PluginEvent(ServerEventType.PlayerInteractScp330)]
        void OnInteractWithScp330(KGPlayer plr)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) interacted with SCP330.");
        }

        [PluginEvent(ServerEventType.RagdollSpawn)]
        void OnRagdollSpawn(KGPlayer plr, IRagdollRole ragdoll, DamageHandlerBase damageHandler)
        {
            // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) spawned ragdoll &6{ragdoll.Ragdoll}&r, reason &6{damageHandler}&r");
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            // Log.Info($"Round ended. {leadingTeam.ToString()} won!");
            Dclass.Clear();
            Scientist.Clear();
            MTF.Clear();
            SCP.Clear();
            CI.Clear();
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRestart()
        {
            TPS.Stop();
        }

        SystemCommandSender cSender = new SystemCommandSender();

        RoleTypeId []SoloScpRole = { RoleTypeId.Scp049 , RoleTypeId.Scp939, RoleTypeId.Scp106 };

        [PluginEvent(ServerEventType.RoundStart)]
        void OnRoundStart()
        {

            time = 0;
            genCount = 0;
            
            TPS.Start();

            if(KGPlayer.GetPlayers().Count < 10)
            {
                foreach (var plr in KGPlayer.GetPlayers())
                {
                    if (plr.Role == RoleTypeId.Scp173)
                    {
                        plr.Role = SoloScpRole.RandomItem();
                    }
                }
            }


        }

        private void TPS_Elapsed(object sender, ElapsedEventArgs e)
        {
            time++;
            timeStr1 = TimeSpan.FromSeconds(time).ToString(@"mm");
            timeStr2 = TimeSpan.FromSeconds(time).ToString(@"ss");

            if(isEscaped && !Warhead.IsDetonationInProgress && time >= 1200)
            {
                Warhead.Start(true);
            }

            var titleStr = $"KG SCP:SL  <color=#C0C0C0>{Player.GetPlayers().Count} / {Server.MaxPlayers} 명</color> | <color=lightblue>{timeStr1}분 {timeStr2}초</color>";

            //ServerConfigSynchronizer.Singleton.NetworkServerName = titleStr;
            Server.RunCommand($"/setconfig player_list_title {titleStr}", cSender);
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        void WaitingForPlayers()
        {
             Log.Info($"Waiting for players...");
        }
    }
}