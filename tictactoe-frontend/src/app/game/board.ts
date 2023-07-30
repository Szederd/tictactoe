
export interface Coordinate {
  x: number;
  y: number;
}

export interface Player {
  user: {
    id: string;
    userName: string;
    isAvailable: boolean;
  };
  mark: Mark;
}

export enum Mark {
  X = 0,
  O = 1,
}


export interface BoardCell {
  mark: number;
  coord: Coordinate;
}
export enum GameStatus {
  Ongoing = 0,
  Over = 1
}


export interface Game {
  gameStatus: GameStatus;
  board: Board;
  nextPlayer: string;
}


interface Board {
  groupName: string;
  boardCells: BoardCell[];
  player1: Player;
  player2: Player;
  winner: Player | null;
}

