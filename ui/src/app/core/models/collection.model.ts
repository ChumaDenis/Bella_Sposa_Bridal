export interface CollectionDto {
  id: string;
  name: string;
  season: string | null;
  year: number;
  description: string | null;
  coverImageUrl: string | null;
  isActive: boolean;
}
