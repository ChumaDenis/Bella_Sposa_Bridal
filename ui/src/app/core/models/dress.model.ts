import { CollectionDto } from './collection.model';

export interface DressListDto {
  id: string;
  name: string;
  slug: string;
  tagline: string;
  silhouette: number;
  silhouetteName: string;
  color: string;
  heroImageUrl: string | null;
  isActive: boolean;
  isDeleted: boolean;
  deletedAt: string | null;
  isHomepageFeatured: boolean;
  homepageFeaturedOrder: number;
  collectionNames: string[];
  sizes: string[];
  createdAt: string;
}

export interface NavDressItem {
  id: string;
  name: string;
  slug: string;
  heroImageUrl: string | null;
  navOrder: number | null;
  isActive: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface DressFilterMeta {
  silhouettes: number[];
  sizes: string[];
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
  type: number;
  order: number;
}

export interface DressVideo {
  id: string;
  url: string;
  thumbnailUrl: string | null;
  type: number;
}

export interface DressDetailDto {
  id: string;
  name: string;
  slug: string;
  tagline: string;
  description: string;
  silhouette: number;
  silhouetteName: string;
  material: string;
  corsetType: string;
  trainDescription: string | null;
  color: string;
  hasSleeves: boolean;
  sleeveDescription: string | null;
  decoration: string | null;
  customTailoringAvailable: boolean;
  isActive: boolean;
  isHomepageFeatured: boolean;
  homepageFeaturedOrder: number;
  createdAt: string;
  updatedAt: string;
  collectionNames: string[];
  collectionIds: string[];
  photos: DressPhoto[];
  videos: DressVideo[];
  sizes: string[];
  relatedDresses: DressListDto[];
}
