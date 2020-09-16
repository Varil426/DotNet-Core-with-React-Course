import React from "react";
import { Tab } from "semantic-ui-react";
import ProfileDescription from "./ProfileDescription";
import ProfilePhotos from "./ProfilePhotos";

const panes = [
	{ menuItem: "About", render: () => <ProfileDescription /> },
	{
		menuItem: "Photos",
		render: () => <ProfilePhotos />,
	},
	{
		menuItem: "Activities",
		render: () => <Tab.Pane>About activities content</Tab.Pane>,
	},
	{
		menuItem: "Followers",
		render: () => <Tab.Pane>About followers contnet</Tab.Pane>,
	},
	{
		menuItem: "Following",
		render: () => <Tab.Pane>About following contnet</Tab.Pane>,
	},
];

const ProfileContent = () => {
	return (
		<Tab
			menu={{ fluid: true, vertical: true }}
			menuPosition="right"
			panes={panes}
		/>
	);
};

export default ProfileContent;
