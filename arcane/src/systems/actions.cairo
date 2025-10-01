use starknet::{ContractAddress};
use core::poseidon;
use dojo_starter::models::{
    PlayerState, PlayerFSMState, PlayerSpellbook, SpellCore, SpellInstance, 
    GameSession, PlayerStats, Vec2i, SpellElement, AttackSubtype
};

// Define the interface
#[starknet::interface]
pub trait IActions<T> {
    fn spawn_player(ref self: T);
    fn update_player_state(
        ref self: T, 
        new_state: PlayerFSMState, 
        pos_x: i32, 
        pos_y: i32, 
        facing_dir: i16, 
        velocity: i16
    );
    fn create_spell(
        ref self: T,
        spell_id: felt252,
        element: SpellElement,
        attack_subtype: AttackSubtype,
        damage: u16,
        knockback: u16,
        projectile_speed: u16,
        projectile_size: u16,
        number_of_projectiles: u8,
        staggered_angle: i16,
        zigzag_amplitude: u16,
        zigzag_frequency: u16,
        homing_radius: u16,
        arc_gravity: u16,
        random_offset: u16,
        circular_speed: u16,
        circular_radius: u16,
        mana_cost: u16,
        cooldown: u16
    );
    fn fire_spell(
        ref self: T, 
        spell_id: felt252, 
        origin_x: i32, 
        origin_y: i32, 
        direction: i16
    ) -> felt252;
    fn equip_spell(ref self: T, spell_id: felt252);
    fn create_game_session(ref self: T, player_2: ContractAddress) -> felt252;
    fn take_damage(ref self: T, damage: u16);
}

// Dojo contract
#[dojo::contract]
pub mod actions {
    use core::poseidon;
use dojo::event::EventStorage;
    use dojo::model::ModelStorage;
    use starknet::{ContractAddress, get_caller_address, get_block_timestamp};
    use super::{
        IActions, PlayerState, PlayerFSMState, PlayerSpellbook, SpellCore, 
        SpellInstance, GameSession, PlayerStats, Vec2i, SpellElement, AttackSubtype
    };

    // Events
    #[derive(Copy, Drop, Serde)]
    #[dojo::event]
    pub struct PlayerSpawned {
        #[key]
        pub player: ContractAddress,
        pub timestamp: u64,
    }

    #[derive(Copy, Drop, Serde)]
    #[dojo::event]
    pub struct PlayerStateUpdated {
        #[key]
        pub player: ContractAddress,
        pub new_state: PlayerFSMState,
        pub position: Vec2i,
    }

    #[derive(Copy, Drop, Serde)]
    #[dojo::event]
    pub struct SpellFired {
        #[key]
        pub caster: ContractAddress,
        pub instance_id: felt252,
        pub spell_id: felt252,
        pub direction: i16,
    }

    #[derive(Copy, Drop, Serde)]
    #[dojo::event]
    pub struct SpellCreated {
        #[key]
        pub creator: ContractAddress,
        pub spell_id: felt252,
        pub element: SpellElement,
    }

    #[derive(Copy, Drop, Serde)]
    #[dojo::event]
    pub struct GameSessionCreated {
        #[key]
        pub session_id: felt252,
        pub player_1: ContractAddress,
        pub player_2: ContractAddress,
    }

    #[abi(embed_v0)]
    impl ActionsImpl of IActions<ContractState> {
        fn spawn_player(ref self: ContractState) {
            let mut world = self.world_default();
            let player = get_caller_address();
            let timestamp = get_block_timestamp();

            // Initialize player state
            let player_state = PlayerState {
                player,
                state: PlayerFSMState::Idle,
                facing_dir: 0,
                position: Vec2i { x: 0, y: 0 },
                velocity: 0,
                last_updated: timestamp,
                is_alive: true,
            };

            world.write_model(@player_state);

            // Initialize player stats
            let player_stats = PlayerStats {
                player,
                health: 100,
                max_health: 100,
                mana: 100,
                max_mana: 100,
                spell_damage_dealt: 0,
                spells_fired: 0,
            };

            world.write_model(@player_stats);

            // Initialize empty spellbook
            let spellbook = PlayerSpellbook {
                player,
                equipped_spells: array![],
                active_spell_index: 0,
                cooldowns: array![],
            };

            world.write_model(@spellbook);

            // Emit event
            world.emit_event(@PlayerSpawned { player, timestamp });
        }

        fn update_player_state(
            ref self: ContractState,
            new_state: PlayerFSMState,
            pos_x: i32,
            pos_y: i32,
            facing_dir: i16,
            velocity: i16
        ) {
            let mut world = self.world_default();
            let player = get_caller_address();
            
            // Read existing state
            let mut state: PlayerState = world.read_model(player);
            
            // Update values
            state.state = new_state;
            state.position = Vec2i { x: pos_x, y: pos_y };
            state.facing_dir = facing_dir;
            state.velocity = velocity;
            state.last_updated = get_block_timestamp();

            // Write back to world
            world.write_model(@state);

            // Emit event
            world.emit_event(@PlayerStateUpdated { 
                player, 
                new_state, 
                position: Vec2i { x: pos_x, y: pos_y } 
            });
        }

        fn create_spell(
            ref self: ContractState,
            spell_id: felt252,
            element: SpellElement,
            attack_subtype: AttackSubtype,
            damage: u16,
            knockback: u16,
            projectile_speed: u16,
            projectile_size: u16,
            number_of_projectiles: u8,
            staggered_angle: i16,
            zigzag_amplitude: u16,
            zigzag_frequency: u16,
            homing_radius: u16,
            arc_gravity: u16,
            random_offset: u16,
            circular_speed: u16,
            circular_radius: u16,
            mana_cost: u16,
            cooldown: u16
        ) {
            let mut world = self.world_default();
            let creator = get_caller_address();

            let spell = SpellCore {
                spell_id,
                creator,
                element,
                attack_subtype,
                damage,
                knockback,
                projectile_speed,
                projectile_size,
                number_of_projectiles,
                staggered_angle,
                zigzag_amplitude,
                zigzag_frequency,
                homing_radius,
                arc_gravity,
                random_offset,
                circular_speed,
                circular_radius,
                mana_cost,
                cooldown,
            };

            world.write_model(@spell);

            world.emit_event(@SpellCreated { creator, spell_id, element });
        }

        fn fire_spell(
            ref self: ContractState,
            spell_id: felt252,
            origin_x: i32,
            origin_y: i32,
            direction: i16
        ) -> felt252 {
            let mut world = self.world_default();
            let caster = get_caller_address();
            let timestamp = get_block_timestamp();

            // Read spell core data
            let spell_core: SpellCore = world.read_model(spell_id);

            // Generate unique instance ID
            let instance_id = self.generate_spell_instance_id(caster, timestamp, spell_id);

            // Create spell instance
            let spell_instance = SpellInstance {
                instance_id,
                spell_id,
                caster,
                origin: Vec2i { x: origin_x, y: origin_y },
                direction,
                timestamp,
                damage: spell_core.damage,
                speed: spell_core.projectile_speed,
                lifetime: 5000, // 5 seconds default
                is_active: true,
            };

            world.write_model(@spell_instance);

            // Update player stats
            let mut stats: PlayerStats = world.read_model(caster);
            stats.spells_fired += 1;
            world.write_model(@stats);

            // Emit event
            world.emit_event(@SpellFired { caster, instance_id, spell_id, direction });

            instance_id
        }

        fn equip_spell(ref self: ContractState, spell_id: felt252) {
            let mut world = self.world_default();
            let player = get_caller_address();

            let mut spellbook: PlayerSpellbook = world.read_model(player);
            
            // Add spell to equipped spells if not already there
            let mut found = false;
            let mut i = 0;
            loop {
                if i >= spellbook.equipped_spells.len() {
                    break;
                }
                if *spellbook.equipped_spells.at(i) == spell_id {
                    found = true;
                    break;
                }
                i += 1;
            };

            if !found {
                spellbook.equipped_spells.append(spell_id);
                spellbook.cooldowns.append(0);
            }

            world.write_model(@spellbook);
        }

        fn create_game_session(ref self: ContractState, player_2: ContractAddress) -> felt252 {
            let mut world = self.world_default();
            let player_1 = get_caller_address();
            let timestamp = get_block_timestamp();

            // Generate session ID
            let session_id = self.generate_session_id(player_1, player_2, timestamp);

            let session = GameSession {
                session_id,
                player_1,
                player_2,
                started_at: timestamp,
                is_active: true,
            };

            world.write_model(@session);

            world.emit_event(@GameSessionCreated { session_id, player_1, player_2 });

            session_id
        }

        fn take_damage(ref self: ContractState, damage: u16) {
            let mut world = self.world_default();
            let player = get_caller_address();

            let mut stats: PlayerStats = world.read_model(player);
            
            if stats.health > damage {
                stats.health -= damage;
            } else {
                stats.health = 0;
                
                // Update player state to dead
                let mut state: PlayerState = world.read_model(player);
                state.state = PlayerFSMState::Dead;
                state.is_alive = false;
                world.write_model(@state);
            }

            world.write_model(@stats);
        }
    }

    #[generate_trait]
    impl InternalImpl of InternalTrait {
        fn world_default(self: @ContractState) -> dojo::world::WorldStorage {
            self.world(@"arcane")
        }

        fn generate_spell_instance_id(
            self: @ContractState, 
            caster: ContractAddress, 
            timestamp: u64,
            spell_id: felt252
        ) -> felt252 {
            let mut data: Array<felt252> = array![];
            data.append(caster.into());
            data.append(timestamp.into());
            data.append(spell_id);
            
            poseidon::poseidon_hash_span(data.span())
        }

        fn generate_session_id(
            self: @ContractState,
            player_1: ContractAddress,
            player_2: ContractAddress,
            timestamp: u64
        ) -> felt252 {
            let mut data: Array<felt252> = array![];
            data.append(player_1.into());
            data.append(player_2.into());
            data.append(timestamp.into());
            
            poseidon::poseidon_hash_span(data.span())
        }
    }
}