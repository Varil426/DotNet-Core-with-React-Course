export interface IActivity {
	id: string;
	title: string;
	description: string;
	date: Date;
	city: string;
	venue: string;
	category: string;
	attendees: IAttendee[];
	isGoing: boolean;
	isHost: boolean;
	comments: IComment[];
}

export interface IComment {
	id: string;
	createdAt: Date;
	body: string;
	username: string;
	displayName: string;
	image: string;
}

export interface IActivityFormValues extends Partial<IActivity> {
	time?: Date;
}

export interface IAttendee {
	username: string;
	displayName: string;
	image: string;
	isHost: boolean;
}

export class ActivityFormValues implements IActivityFormValues {
	id?: string = undefined;
	title: string = "";
	category: string = "";
	description: string = "";
	city: string = "";
	date?: Date = undefined;
	time?: Date = undefined;
	venue: string = "";

	constructor(init?: IActivityFormValues) {
		if (init && init.date) {
			init.time = init.date;
		}
		Object.assign(this, init);
	}
}
