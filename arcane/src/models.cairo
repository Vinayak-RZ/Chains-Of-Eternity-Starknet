use starknet::ContractAddress;

// Player FSM States
#[derive(Serde, Copy, Drop, Introspect, PartialEq, Debug, DojoStore, Default)]
pub enum PlayerFSMState {
    #[default]
    Idle,
    Running,
    Dashing,
    Attacking,
    Dead,
}

// Spell Elements
#[derive(Serde, Copy, Drop, Introspect, PartialEq, Debug, DojoStore, Default)]
pub enum SpellElement {
    #[default]
    Fire,
    Water,
    Lightning,
    Wind,
}

// Attack Subtypes
#[derive(Serde, Copy, Drop, Introspect, PartialEq, Debug, DojoStore, Default)]
pub enum AttackSubtype {
    #[default]
    Projectile,
    AoE,
}

// Position Vector
#[derive(Copy, Drop, Serde, IntrospectPacked, Debug, DojoStore)]
pub struct Vec2i {
    pub x: i32,
    pub y: i32,
}

// Player State Component
#[derive(Copy, Drop, Serde, Debug)]
#[dojo::model]
pub struct PlayerState {
    #[key]
    pub player: ContractAddress,
    pub state: PlayerFSMState,
    pub facing_dir: i16,        // degrees * 100
    pub position: Vec2i,
    pub velocity: i16,
    pub last_updated: u64,
    pub is_alive: bool,
}

// Player Spellbook Component
#[derive(Drop, Serde, Debug)]
#[dojo::model]
pub struct PlayerSpellbook {
    #[key]
    pub player: ContractAddress,
    pub equipped_spells: Array<felt252>,
    pub active_spell_index: u8,
    pub cooldowns: Array<u64>,
}

// Spell Core Definition Component
#[derive(Copy, Drop, Serde, Debug)]
#[dojo::model]
pub struct SpellCore {
    #[key]
    pub spell_id: felt252,
    pub creator: ContractAddress,
    pub element: SpellElement,
    pub attack_subtype: AttackSubtype,
    pub damage: u16,
    pub knockback: u16,
    pub projectile_speed: u16,
    pub projectile_size: u16,
    pub number_of_projectiles: u8,
    pub staggered_angle: i16,
    pub zigzag_amplitude: u16,
    pub zigzag_frequency: u16,
    pub homing_radius: u16,
    pub arc_gravity: u16,
    pub random_offset: u16,
    pub circular_speed: u16,
    pub circular_radius: u16,
    pub mana_cost: u16,
    pub cooldown: u16,
}

// Spell Instance Component (fired spells)
#[derive(Copy, Drop, Serde, Debug)]
#[dojo::model]
pub struct SpellInstance {
    #[key]
    pub instance_id: felt252,
    pub spell_id: felt252,
    pub caster: ContractAddress,
    pub origin: Vec2i,
    pub direction: i16,
    pub timestamp: u64,
    pub damage: u16,
    pub speed: u16,
    pub lifetime: u16,
    pub is_active: bool,
}

// Game Session Tracker
#[derive(Copy, Drop, Serde, Debug)]
#[dojo::model]
pub struct GameSession {
    #[key]
    pub session_id: felt252,
    pub player_1: ContractAddress,
    pub player_2: ContractAddress,
    pub started_at: u64,
    pub is_active: bool,
}

// Player Stats
#[derive(Copy, Drop, Serde, Debug)]
#[dojo::model]
pub struct PlayerStats {
    #[key]
    pub player: ContractAddress,
    pub health: u16,
    pub max_health: u16,
    pub mana: u16,
    pub max_mana: u16,
    pub spell_damage_dealt: u32,
    pub spells_fired: u32,
}

// Trait implementations for enums
impl PlayerFSMStateIntoU8 of Into<PlayerFSMState, u8> {
    fn into(self: PlayerFSMState) -> u8 {
        match self {
            PlayerFSMState::Idle => 0,
            PlayerFSMState::Running => 1,
            PlayerFSMState::Dashing => 2,
            PlayerFSMState::Attacking => 3,
            PlayerFSMState::Dead => 4,
        }
    }
}

impl SpellElementIntoU8 of Into<SpellElement, u8> {
    fn into(self: SpellElement) -> u8 {
        match self {
            SpellElement::Fire => 0,
            SpellElement::Water => 1,
            SpellElement::Lightning => 2,
            SpellElement::Wind => 3,
        }
    }
}

impl AttackSubtypeIntoU8 of Into<AttackSubtype, u8> {
    fn into(self: AttackSubtype) -> u8 {
        match self {
            AttackSubtype::Projectile => 0,
            AttackSubtype::AoE => 1,
        }
    }
}

// Helper trait for Vec2i
#[generate_trait]
impl Vec2iImpl of Vec2iTrait {
    fn is_zero(self: Vec2i) -> bool {
        self.x == 0 && self.y == 0
    }

    fn is_equal(self: Vec2i, other: Vec2i) -> bool {
        self.x == other.x && self.y == other.y
    }

    fn distance_squared(self: Vec2i, other: Vec2i) -> u64 {
        let dx = if self.x > other.x { self.x - other.x } else { other.x - self.x };
        let dy = if self.y > other.y { self.y - other.y } else { other.y - self.y };
        (dx * dx + dy * dy).try_into().unwrap()
    }
}

#[cfg(test)]
mod tests {
    use super::{Vec2i, Vec2iTrait, PlayerFSMState};

    #[test]
    fn test_vec2i_is_zero() {
        assert(Vec2iTrait::is_zero(Vec2i { x: 0, y: 0 }), 'not zero');
    }

    #[test]
    fn test_vec2i_is_equal() {
        let pos = Vec2i { x: 100, y: 200 };
        assert(pos.is_equal(Vec2i { x: 100, y: 200 }), 'not equal');
    }
}