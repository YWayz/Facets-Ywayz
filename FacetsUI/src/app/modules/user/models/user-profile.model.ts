export class UserProfileModel {
    id: string;
    email: string;
    userName: string;
    firstName: string;
    lastName: string;
    timeZone: string;
    imageURL: string = '../../assets/images/no-image.png';
    roles: string[];

    setImageUrl() {
        this.imageURL = '../../assets/images/no-image.png';
    }
}