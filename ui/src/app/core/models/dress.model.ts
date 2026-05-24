import { CollectionDto } from './collection.model';

export interface DressListDto {
  id: string;
  name: string;
  tagline: string;
  silhouette: string;
  color: string;
  heroImageUrl: string | null;
  collectionNames: string[];
}

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
  silhouette: string;
  material: string;
  corsetType: string;
  trainDescription: string | null;
  color: string;
  hasSleeves: boolean;
  sleeveDescription: string | null;
  decoration: string | null;
  customTailoringAvailable: boolean;
  collections: CollectionDto[];
  photos: DressPhoto[];
  videos: DressVideo[];
  sizes: string[];
  relatedDresses: DressListDto[];
}
