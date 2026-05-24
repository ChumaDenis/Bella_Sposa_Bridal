import { CollectionDto } from './collection.model';

export interface DressListDto {
  id: string;
  name: string;
  tagline: string;
  silhouette: number;
  color: string;
  heroImageUrl: string | null;
  collectionNames: string[];
}

export const SILHOUETTE_LABELS: Record<number, string> = {
  0: 'Mermaid',
  1: 'Ball Gown',
  2: 'A-Line',
  3: 'Sheath',
  4: 'Empire',
  5: 'Trumpet',
  6: 'Tea Length',
  7: 'Mini',
};

export interface DressPhoto {
  id: string;
  url: string;
  altText: string | null;
  type: string;
  order: number;
}

export interface DressVideo {
  id: string;
  url: string;
  thumbnailUrl: string | null;
  type: string;
}

export interface DressDetailDto {
  id: string;
  name: string;
  tagline: string;
  description: string;
  silhouette: number;
  material: string;
  corsetType: string;
  trainDescription: string | null;
  color: string;
  hasSleeves: boolean;
  sleeveDescription: string | null;
  decoration: string | null;
  customTailoringAvailable: boolean;
  collectionNames: string[];
  photos: DressPhoto[];
  videos: DressVideo[];
  sizes: string[];
  relatedDresses: DressListDto[];
}
